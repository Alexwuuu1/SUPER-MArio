using UnityEngine;
public class MarioWorldMap : MonoBehaviour {
 public Texture2D mapPlayer,mapCastle,mapPipe;
 private Texture2D mapLandscape;
    private readonly Vector2[] points={new Vector2(150,320),new Vector2(355,245),new Vector2(575,330),new Vector2(790,235)};
    private int selected=-1, visualNode;
    private Vector2 marker;
    private float travel;
    private bool moving;
    private string previousScene;
    private void Update(){
        string scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var gm=GameManager.Instance;
        if(scene!="Map" || gm==null){previousScene=scene;return;}
        if(previousScene!="Map"){selected=Mathf.Clamp(gm.stage-1,0,3);visualNode=selected;marker=points[selected];moving=false;}
        previousScene=scene;
        if(Input.GetKeyDown(KeyCode.RightArrow)||Input.GetKeyDown(KeyCode.D))Select(Mathf.Min(gm.unlockedStage-1,selected+1));
        if(Input.GetKeyDown(KeyCode.LeftArrow)||Input.GetKeyDown(KeyCode.A))Select(Mathf.Max(0,selected-1));
        if(moving){travel+=Time.unscaledDeltaTime*2.2f;marker=Vector2.Lerp(points[visualNode],points[selected],Mathf.SmoothStep(0,1,travel));if(travel>=1){moving=false;visualNode=selected;}}
        if(!moving&&Input.GetKeyDown(KeyCode.Return))gm.LoadLevel(1,selected+1);
    }
    private void Select(int node){if(moving||selected==node)return;selected=node;travel=0;moving=true;MarioAudio.Play("coin");}
    public void Draw(GameManager gm, GUIStyle title, GUIStyle text, GUIStyle button, System.Action selectLevels) {
        if(selected<0){selected=Mathf.Clamp(gm.stage-1,0,3);visualNode=selected;marker=points[selected];}
        if(mapLandscape==null)mapLandscape=MakeLandscape();
        GUI.DrawTexture(new Rect(0,0,960,540),mapLandscape);
        for(int tree=0;tree<8;tree++)GUI.DrawTexture(new Rect(74+tree*43,154+(tree%3)*30,35,48),WorldMapSprites.Get(0));
        GUI.DrawTexture(new Rect(310,153,86,72),WorldMapSprites.Get(1));
        GUI.DrawTexture(new Rect(550,235+Mathf.Sin(Time.unscaledTime)*3,65,65),WorldMapSprites.Get(2));
        GUI.DrawTexture(new Rect(745,136,90,90),WorldMapSprites.Get(3));
        Panel(new Rect(22,20,916,98),new Color(.035f,.10f,.16f,.94f));
        GUI.Label(new Rect(42,25,620,48),"EL REINO CHAMPINON",new GUIStyle(title){fontSize=34,alignment=TextAnchor.MiddleLeft});
        GUI.Label(new Rect(44,74,610,28),$"VIDAS  {gm.lives}     MONEDAS  {gm.coins:00}     PUNTAJE  {gm.score:000000}",new GUIStyle(text){fontSize=17,alignment=TextAnchor.MiddleLeft});
        string[] names={"PRADERA","SUBSUELO","OCEANO","LA FORJA"};

        for(int i=0;i<3;i++){
            Vector2 d=points[i+1]-points[i];int dots=60;
            for(int j=0;j<dots;j++){float t=j/(dots-1f);var p=points[i]+d*t+new Vector2(0,Mathf.Sin(t*Mathf.PI)*(i==1?25:-20));Panel(new Rect(p.x-7,p.y-7,14,14),new Color(.28f,.23f,.13f));if(j%5<3)Panel(new Rect(p.x-3,p.y-3,6,6),i<gm.unlockedStage-1?new Color(1,.87f,.43f):new Color(.62f,.66f,.52f));}
        }
        GUI.color=Color.white;
        for(int i=0;i<4;i++){
            var p=points[i];bool open=i<gm.unlockedStage;GUI.enabled=open;
            GUI.backgroundColor=open?new Color(1,.82f,.33f):new Color(.4f,.5f,.46f);
            if(GUI.Button(new Rect(p.x-35,p.y-24,70,52),open?"1-"+(i+1):"?",button)){if(selected==i&&!moving)gm.LoadLevel(1,i+1);else Select(i);}
            GUI.backgroundColor=Color.white;GUI.enabled=true;
            Panel(new Rect(p.x-83,p.y+36,166,29),new Color(.03f,.10f,.13f,.9f));
            GUI.Label(new Rect(p.x-83,p.y+34,166,30),names[i],new GUIStyle(text){fontSize=15,fontStyle=FontStyle.Bold});
            if(i==1&&mapPipe!=null)GUI.DrawTexture(new Rect(p.x-28,p.y-66,56,28),mapPipe,ScaleMode.ScaleToFit,true);
            if(i==3&&mapCastle!=null)GUI.DrawTexture(new Rect(p.x-48,p.y-112,96,88),mapCastle,ScaleMode.ScaleToFit,true);
            
        }
        if(mapPlayer!=null)GUI.DrawTexture(new Rect(marker.x-21,marker.y-95+Mathf.Sin(Time.unscaledTime*8)*(moving?5:2),42,48),mapPlayer,ScaleMode.ScaleToFit,true);
        Panel(new Rect(22,450,916,68),new Color(.035f,.10f,.16f,.95f));
        GUI.Label(new Rect(42,458,560,42),names[Mathf.Clamp(selected,0,3)]+"  /  FLECHAS Y ENTER",new GUIStyle(text){fontSize=16,alignment=TextAnchor.MiddleLeft});
        if(GUI.Button(new Rect(665,461,250,44),"PROBAR NIVELES",button))selectLevels();
        GUI.Label(new Rect(550,405,360,28),"LA FORJA DE BOWSER TE ESPERA",new GUIStyle(text){fontSize=13,fontStyle=FontStyle.Bold});
        GUI.Label(new Rect(750,35,180,24),"MUSICA  [M]",new GUIStyle(text){fontSize=13});
        MarioAudio.MusicVolume=GUI.HorizontalSlider(new Rect(780,68,130,15),MarioAudio.MusicVolume,0,1);
    }
    private static void Panel(Rect rect,Color color){GUI.color=color;GUI.DrawTexture(rect,Texture2D.whiteTexture);GUI.color=Color.white;}
    private static Texture2D MakeLandscape(){
        var texture=new Texture2D(320,180,TextureFormat.RGBA32,false);texture.filterMode=FilterMode.Point;
        for(int y=0;y<180;y++)for(int x=0;x<320;x++){
            Color c=Color.Lerp(new Color(.12f,.52f,.69f),new Color(.04f,.31f,.49f),y/180f);
            if((y+x/15)%17==0)c=new Color(.2f,.61f,.73f);
            float nx=(x-158)/143f,ny=(y-102)/55f;float island=nx*nx+ny*ny;
            if(island<1.12f)c=new Color(.14f,.35f,.33f);
            if(island<1.04f)c=new Color(.92f,.82f,.50f);
            if(island<.96f){c=new Color(.36f,.65f,.27f);if((x*7+y*13)%67<3)c=new Color(.49f,.74f,.31f);if(y>130)c=new Color(.26f,.51f,.24f);}
            float lakeX=(x-193)/29f,lakeY=(y-112)/32f;
            if(island<.96f&&lakeX*lakeX+lakeY*lakeY<1)c=(y%9==0)?new Color(.35f,.77f,.84f):new Color(.09f,.48f,.68f);
            if(x>238&&x<292&&y>53&&y<100&&Mathf.Abs(x-265)<(y-53)*.65f)c=x<265?new Color(.38f,.40f,.39f):new Color(.25f,.29f,.31f);
            if(x>249&&x<280&&y>57&&y<71&&Mathf.Abs(x-265)<(y-53)*.65f)c=new Color(.8f,.82f,.7f);
            if(island<.88f&&lakeX*lakeX+lakeY*lakeY>1.1f&&(x*19+y*7)%197==0)c=new Color(1,.85f,.35f);
            texture.SetPixel(x,179-y,c);
        }
        texture.Apply();return texture;
    }
}
