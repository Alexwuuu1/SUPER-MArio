using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class CollaborationChecks
{
    private static int step;
    private static double next;
    private static Vector3 pausedPosition;
    private static float pausedTime;
    private static int previousScore;
    private static MarioFirePower fire;
    private static bool runtimeError;
    static CollaborationChecks(){EditorApplication.playModeStateChanged+=Changed;}
    public static void Run(){SessionState.SetBool("CollaborationChecks",true);EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");EditorApplication.EnterPlaymode();}
    private static void Changed(PlayModeStateChange s){
        if(s!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool("CollaborationChecks",false))return;
        step=0;runtimeError=false;next=EditorApplication.timeSinceStartup+2;EditorApplication.update+=Tick;Application.logMessageReceived+=Log;
    }
    private static void Log(string text,string stack,LogType type){if((type==LogType.Error||type==LogType.Exception)&&!stack.Contains("UnityEditor.Search"))runtimeError=true;}
    private static void Check(bool ok,string message){if(!ok)throw new Exception(message);Debug.Log("COLLAB_OK: "+message);}
    private static void Tick(){
        if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.8;
        try {
            var gm=GameManager.Instance;
            switch(step++){
                case 0:Check(gm!=null,"sesion inicial");gm.StartTestLevel(2);break;
                case 1:
                    Check(UnityEngine.Object.FindFirstObjectByType<MarioPauseMenu>()!=null,"pausa instalada");
                    fire=UnityEngine.Object.FindFirstObjectByType<Player>().GetComponent<MarioFirePower>();Check(fire!=null,"poder instalado en jugador");
                    fire.Grant();break;
                case 2:
                    Check(fire.active&&fire.GetComponent<Player>().big,"transformacion persiste tras animacion");
                    pausedPosition=fire.transform.position;pausedTime=gm.timeRemaining;MarioPauseMenu.SetPaused(true);break;
                case 3:
                    Check(Time.timeScale==0&&AudioListener.pause,"pausa detiene tiempo y audio");
                    Check(Vector3.Distance(pausedPosition,fire.transform.position)<.001f&&Mathf.Abs(pausedTime-gm.timeRemaining)<.001f,"pausa conserva jugador y temporizador");
                    Check(!fire.TryShoot(1),"no disparar durante pausa");MarioPauseMenu.SetPaused(false);break;
                case 4:
                    Check(Time.timeScale==1&&!AudioListener.pause,"continuar restaura tiempo y audio");
                    Check(fire.TryShoot(1),"disparo hacia derecha");Check(!fire.TryShoot(1),"intervalo evita disparos instantaneos");
                    var cooldown=typeof(MarioFirePower).GetField("nextShot",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
                    cooldown.SetValue(fire,0f);Check(fire.TryShoot(-1),"segundo disparo hacia izquierda");cooldown.SetValue(fire,0f);Check(!fire.TryShoot(1),"maximo dos bolas activas");break;
                case 5:
                    fire.GetComponent<Player>().Hit();Check(!fire.active&&fire.GetComponent<Player>().big,"daño quita fuego y mantiene Mario grande");
                    gm.LoadLevel(1,1);break;
                case 6:
                    previousScore=gm.score;
                    var player=UnityEngine.Object.FindFirstObjectByType<Player>();
                    // Isolate the projectile test from the level terrain.
                    player.GetComponent<PlayerMovement>().enabled=false;player.GetComponent<Collider2D>().enabled=true;
                    player.transform.position=new Vector3(300,30);
                    var go=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Goomba.prefab"),new Vector3(302,30.4f),Quaternion.identity);
                    go.GetComponent<EntityMovement>().enabled=false;go.GetComponent<Rigidbody2D>().bodyType=RigidbodyType2D.Kinematic;
                    MarioFireball.Launch(player,1);break;
                case 7:
                    Check(gm.score==previousScore+100,"bola de fuego elimina Goomba y suma una sola vez");
                    gm.LoadLevel(1,2);break;
                case 8:
                    MarioPauseMenu.SetPaused(true);gm.LoadLevel(1,3);break;
                case 9:
                    Check(!MarioPauseMenu.IsPaused&&Time.timeScale==1,"cambio de escena desde pausa restaura partida");
                    Check(UnityEngine.Object.FindFirstObjectByType<WaterMotion>()!=null,"nado compatible");
                    gm.NewGame();break;
                case 10:
                    Check(gm.completedMask==0&&gm.unlockedStage==1,"partida nueva limpia progreso");gm.LoadLevel(1,1);break;
                case 11:gm.NextLevel();break;
                case 12:
                    Check(SceneManager.GetActiveScene().name=="Map"&&gm.IsComplete(1)&&gm.unlockedStage==2,"mapa registra completado y desbloqueo");
                    Check(!runtimeError,"sin excepciones de runtime");Finish(true);break;
            }
        }catch(Exception error){Debug.LogError(error);Finish(false);}
    }
    private static void Finish(bool pass){SessionState.SetBool("CollaborationChecks",false);EditorApplication.update-=Tick;Application.logMessageReceived-=Log;MarioPauseMenu.SetPaused(false);File.WriteAllText("../colaboraciones-verificacion.txt",pass?"PASS - pausa, audio, flor, disparos, daño, enemigos y progreso":"FAILED - revisar colaboraciones-checks.log");Debug.Log(pass?"COLLABORATION_CHECKS_OK":"COLLABORATION_CHECKS_FAILED");EditorApplication.Exit(pass?0:1);}
}
