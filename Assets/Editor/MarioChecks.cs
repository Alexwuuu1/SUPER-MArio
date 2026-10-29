using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class MarioChecks
{
    private static int step;
    private static double next;
    private static bool failed;
    static MarioChecks(){EditorApplication.playModeStateChanged+=State;}
    public static void Run(){
        SessionState.SetBool("MarioChecks",true);
        EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");
        EditorApplication.EnterPlaymode();
    }
    private static void State(PlayModeStateChange state){
        if(state!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool("MarioChecks",false))return;
        step=0;next=EditorApplication.timeSinceStartup+2;failed=false;
        Application.logMessageReceived+=Log;
        EditorApplication.update+=Tick;
    }
    private static void Log(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error){if(stack.Contains("UnityEditor.Search.")){File.AppendAllText("../mario-editor-errors.txt",message+"\n"+stack+"\n");return;}failed=true;File.AppendAllText("../mario-runtime-errors.txt",message+"\n"+stack+"\n");}}
    private static void Require(bool condition,string message){if(!condition)throw new Exception(message);Debug.Log("CHECK_OK: "+message);}
    private static void Snapshot(string name){
        Directory.CreateDirectory("../Mario-Capturas");
        var cam=Camera.main;var rt=new RenderTexture(1280,720,24);var old=cam.targetTexture;cam.targetTexture=rt;cam.Render();
        var oldActive=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(1280,720,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1280,720),0,0);image.Apply();
        File.WriteAllBytes("../Mario-Capturas/"+name+".png",image.EncodeToPNG());cam.targetTexture=old;RenderTexture.active=oldActive;UnityEngine.Object.DestroyImmediate(image);UnityEngine.Object.DestroyImmediate(rt);
    }
    private static void Tick(){
        if(EditorApplication.timeSinceStartup<next)return;
        next=EditorApplication.timeSinceStartup+1.2;
        try{
            var gm=GameManager.Instance;
            switch(step++){
                case 0:Require(gm!=null&&SceneManager.GetActiveScene().name=="Menu","menu y administrador activos");gm.NewGame();break;
                case 1:Require(SceneManager.GetActiveScene().name=="Map"&&gm.unlockedStage==1,"mapa inicial y primer nivel desbloqueado");Require(MarioAudio.MusicPlaying&&MarioAudio.MusicVolume>.1f,"musica ambiente reproduciendose con volumen audible");gm.LoadLevel(1,1);break;
                case 2:Require(UnityEngine.Object.FindFirstObjectByType<Player>()!=null,"jugador en nivel 1-1");Snapshot("01-superficie");gm.NextLevel();break;
                case 3:Require(SceneManager.GetActiveScene().name=="Map"&&gm.unlockedStage==2,"desbloqueo del subsuelo");gm.LoadLevel(1,2);break;
                case 4:Require(gm.timeRemaining<400&&gm.timeRemaining>395,"temporizador del subsuelo");Require(UnityEngine.Object.FindFirstObjectByType<LevelPortal>()!=null,"salida del subsuelo");Snapshot("02-subterraneo");var coin=UnityEngine.Object.FindObjectsByType<PowerUp>(FindObjectsSortMode.None).First(c=>c.type==PowerUp.Type.Coin);var p=UnityEngine.Object.FindFirstObjectByType<Player>();p.GetComponent<Rigidbody2D>().position=coin.transform.position;break;
                case 5:Require(gm.coins>0&&gm.score>=200,"recoger moneda suma contador y puntaje");var exit2=UnityEngine.Object.FindFirstObjectByType<LevelPortal>();UnityEngine.Object.FindFirstObjectByType<Player>().GetComponent<Rigidbody2D>().position=exit2.transform.position;break;
                case 6:Require(SceneManager.GetActiveScene().name=="Map"&&gm.unlockedStage==3,"desbloqueo del oceano");gm.LoadLevel(1,3);break;
                case 7:Require(UnityEngine.Object.FindFirstObjectByType<WaterMotion>().GetComponent<Player>()!=null,"mecanica de nado unida al jugador");Require(gm.timeRemaining<400,"temporizador acuatico activo");Snapshot("03-acuatico");var exit3=UnityEngine.Object.FindFirstObjectByType<LevelPortal>();UnityEngine.Object.FindFirstObjectByType<Player>().GetComponent<Rigidbody2D>().position=exit3.transform.position;break;
                case 8:Require(SceneManager.GetActiveScene().name=="Map"&&gm.unlockedStage==4,"desbloqueo del castillo; escena="+SceneManager.GetActiveScene().name+" vidas="+gm.lives);gm.LoadLevel(1,4);break;
                case 9:Require(UnityEngine.Object.FindFirstObjectByType<CastleBoss>()!=null&&UnityEngine.Object.FindFirstObjectByType<CastleAxe>()!=null,"jefe y mecanismo final presentes");Snapshot("04-castillo");var camera=Camera.main;camera.transform.position=new Vector3(112,6,-10);Snapshot("05-puente-bowser");gm.NextLevel();break;
                case 10:Require(SceneManager.GetActiveScene().name=="Victory","victoria al completar castillo");gm.NewGame();break;
                case 11:gm.LoadLevel(1,2);break;
                case 12:gm.ResetLevel();break;
                case 13:Require(gm.lives==2,"primera muerte resta una vida");gm.ResetLevel();break;
                case 14:Require(gm.lives==1,"segunda muerte resta una vida");gm.ResetLevel();break;
                case 15:Require(gm.lives==0&&SceneManager.GetActiveScene().name=="GameOver","game over tras perder todas las vidas");gm.NewGame();break;
                case 16:Require(gm.lives==3&&gm.coins==0&&gm.score==0&&gm.unlockedStage==1,"reinicio limpio de partida");gm.StartTestLevel(4);break;
                case 17:Require(gm.lives==3&&gm.testingLevel&&gm.unlockedStage==4&&SceneManager.GetActiveScene().name=="1-4","selector abre castillo directamente con tres vidas");gm.NewGame();break;
                case 18:ScreenCapture.CaptureScreenshot("../Mario-Capturas/mapa-nuevo.png");break;
                case 19:Finish();break;
            }
        }catch(Exception e){failed=true;Debug.LogError("CHECK_FAILED: "+e);Finish();}
    }
    private static void Finish(){
        SessionState.SetBool("MarioChecks",false);EditorApplication.update-=Tick;Application.logMessageReceived-=Log;
        File.WriteAllText("../mario-verificacion.txt",failed?"FAILED - revisar mario-checks.log":"PASS - menu, mapa, desbloqueos, 4 niveles, monedas, tiempo, vidas, victoria y game over");
        Debug.Log(failed?"MARIO_CHECKS_FAILED":"MARIO_CHECKS_OK");EditorApplication.Exit(failed?1:0);
    }
}
