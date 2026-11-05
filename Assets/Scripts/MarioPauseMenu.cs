using UnityEngine;
using UnityEngine.SceneManagement;

public class MarioPauseMenu : MonoBehaviour
{
    public static bool IsPaused { get; private set; }
    private bool settings, controls;
    private int selection;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install() {
        IsPaused=false;Time.timeScale=1;AudioListener.pause=false;
        if(FindFirstObjectByType<MarioPauseMenu>()==null){var go=new GameObject("Pause and preferences");DontDestroyOnLoad(go);go.AddComponent<MarioPauseMenu>();}
    }
    private void Awake(){SceneManager.sceneLoaded+=Loaded;}
    private void OnDestroy(){SceneManager.sceneLoaded-=Loaded;SetPaused(false);}
    private void Loaded(Scene scene,LoadSceneMode mode){SetPaused(false);settings=controls=false;}
    public static void SetPaused(bool value){IsPaused=value;Time.timeScale=value?0:1;AudioListener.pause=value;}
    private void Update(){
        var gm=GameManager.Instance;
        if(gm==null)return;
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(settings||controls){settings=controls=false;return;}
            if(gm.playing){SetPaused(!IsPaused);selection=0;}
        }
        if(!IsPaused||settings||controls)return;
        if(Input.GetKeyDown(KeyCode.DownArrow))selection=(selection+1)%4;
        if(Input.GetKeyDown(KeyCode.UpArrow))selection=(selection+3)%4;
        if(Input.GetKeyDown(KeyCode.Return))Choose(selection);
    }
    private void Choose(int index){
        var gm=GameManager.Instance;
        if(index==0)SetPaused(false);
        else if(index==1){SetPaused(false);gm.LoadLevel(gm.world,gm.stage);}
        else if(index==2)settings=true;
        else {SetPaused(false);SceneManager.LoadScene("Menu");}
    }
    private void OnGUI(){
        var gm=GameManager.Instance;if(gm==null)return;
        float scale=Mathf.Min(Screen.width/960f,Screen.height/540f);
        GUI.matrix=Matrix4x4.TRS(new Vector3((Screen.width-960*scale)/2,(Screen.height-540*scale)/2),Quaternion.identity,Vector3.one*scale);
        GUI.depth=-50;
        var label=new GUIStyle(GUI.skin.label){fontSize=22,alignment=TextAnchor.MiddleCenter};
        var button=new GUIStyle(GUI.skin.button){fontSize=21};
        if(!gm.playing&&!IsPaused&&!settings&&!controls){
            if(GUI.Button(new Rect(20,15,150,38),"AJUSTES",button))settings=true;
            if(GUI.Button(new Rect(180,15,170,38),"CONTROLES",button))controls=true;
        }
        if(!IsPaused&&!settings&&!controls)return;
        GUI.color=new Color(0,0,0,.82f);GUI.DrawTexture(new Rect(0,0,960,540),Texture2D.whiteTexture);GUI.color=Color.white;
        GUI.Box(new Rect(230,55,500,435),"");
        if(controls){
            GUI.Label(new Rect(250,70,460,45),"CONTROLES",label);
            GUI.Label(new Rect(260,130,440,200),"A / D o flechas: moverse\nESPACIO: saltar o nadar\nX: disparar con flor de fuego\nS: entrar en tuberias\nESC: pausa / volver\nM: silenciar musica",label);
            if(GUI.Button(new Rect(350,395,260,45),"VOLVER",button))controls=false;
        } else if(settings){
            DrawSettings(label,button);
        } else {
            GUI.Label(new Rect(250,65,460,60),"PAUSA",new GUIStyle(label){fontSize=38,fontStyle=FontStyle.Bold});
            string[] items={"CONTINUAR","REINICIAR NIVEL","AJUSTES","VOLVER AL MENU"};
            for(int i=0;i<items.Length;i++)if(GUI.Button(new Rect(290,145+i*69,380,53),(selection==i?">  ":"")+items[i],button))Choose(i);
        }
    }
    private void DrawSettings(GUIStyle label,GUIStyle button){
        GUI.Label(new Rect(250,75,460,55),"AJUSTES",label);
        GUI.Label(new Rect(260,145,440,45),"Musica: "+Mathf.RoundToInt(MarioAudio.MusicVolume*100)+"%",label);
        MarioAudio.MusicVolume=GUI.HorizontalSlider(new Rect(310,200,340,25),MarioAudio.MusicVolume,0,1);
        if(GUI.Button(new Rect(350,395,260,45),"VOLVER",button))settings=false;
    }
}
