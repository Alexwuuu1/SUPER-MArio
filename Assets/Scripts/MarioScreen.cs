using UnityEngine;
using UnityEngine.SceneManagement;

public class MarioScreen : MonoBehaviour
{
    private GUIStyle title, text, button, hud;
    private int selection;
    private bool selectingLevel;
    public Texture2D mapPlayer, mapCastle, mapPipe;
    private Texture2D mapLandscape;
    private string Scene => SceneManager.GetActiveScene().name;
    private void Update()
    {
        if (selectingLevel) { if(Input.GetKeyDown(KeyCode.Escape))selectingLevel=false;return; }
        if (Scene == "Menu") {
            if (Input.GetKeyDown(KeyCode.DownArrow)) { selection = (selection+1)%3; MarioAudio.Play("coin"); }
            if (Input.GetKeyDown(KeyCode.UpArrow)) { selection = (selection+2)%3; MarioAudio.Play("coin"); }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) { if(selection==2)selectingLevel=true;else GameManager.Instance.NewGame(); }
        } else if (Scene=="Map") {
            if (Input.GetKeyDown(KeyCode.Return)) GameManager.Instance.LoadLevel(1,GameManager.Instance.stage);
        } else if (!GameManager.Instance.playing && Input.GetKeyDown(KeyCode.Return)) GameManager.Instance.NewGame();
    }
    private void OnGUI()
    {
        float scale = Mathf.Min(Screen.width / 960f, Screen.height / 540f);
        GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width-960*scale)/2,(Screen.height-540*scale)/2,0), Quaternion.identity, Vector3.one*scale);
        if (title == null) {
            title = new GUIStyle(GUI.skin.label) { fontSize=56, fontStyle=FontStyle.Bold, alignment=TextAnchor.MiddleCenter };
            title.normal.textColor = new Color(1,.82f,.37f);
            text = new GUIStyle(GUI.skin.label) { fontSize=20, alignment=TextAnchor.MiddleCenter };
            text.normal.textColor=Color.white;
            hud = new GUIStyle(text) { fontSize=19, fontStyle=FontStyle.Bold, alignment=TextAnchor.UpperLeft };
            button = new GUIStyle(GUI.skin.button) { fontSize=23, fontStyle=FontStyle.Bold };
        }
        var gm=GameManager.Instance;
        if (gm == null) return;
        GUI.Label(new Rect(750,Scene.StartsWith("1-")?60:10,200,25),"MUSICA  "+Mathf.RoundToInt(MarioAudio.MusicVolume*100)+"%  [M]",new GUIStyle(text){fontSize=14});
        MarioAudio.MusicVolume=GUI.HorizontalSlider(new Rect(805,Scene.StartsWith("1-")?87:38,120,15),MarioAudio.MusicVolume,0,1);
        if (selectingLevel) { DrawSelector(gm); return; }
        if (Scene=="Map") { DrawMap(gm); return; }
        if (gm.playing) {
            GUI.color=new Color(0,0,0,.8f); GUI.DrawTexture(new Rect(0,0,960,58),Texture2D.whiteTexture); GUI.color=Color.white;
            GUI.Label(new Rect(28,10,250,45),$"MARIO   {gm.score:000000}",hud);
            GUI.Label(new Rect(300,10,170,45),$"COINS  {gm.coins:00}",hud);
            GUI.Label(new Rect(500,10,200,45),$"WORLD  {gm.world}-{gm.stage}",hud);
            GUI.Label(new Rect(730,10,220,45),$"TIME {(int)gm.timeRemaining:000}  x{gm.lives}",hud);
            if (Time.timeSinceLevelLoad < 3) GUI.Label(new Rect(0,80,960,35),gm.stage==2 ? "1-2  /  SUBTERRANEO" : gm.stage==3 ? "1-3  /  PROFUNDIDADES  -  ESPACIO PARA NADAR" : gm.stage==4 ? "1-4  /  LA FORJA DE BOWSER" : "1-1  /  REINO CHAMPINON",text);
            if(gm.testingLevel)GUI.Label(new Rect(20,495,500,30),"MODO PRUEBAS  -  NIVEL SELECCIONADO",hud);
            return;
        }
        GUI.Label(new Rect(60,90,840,100),Scene=="Menu" ? "SUPER MARIO" : Scene=="GameOver" ? "GAME OVER" : "REINO SALVADO",title);
        GUI.Label(new Rect(80,190,800,45),Scene=="Menu" ? "UNA AVENTURA A TRAVES DEL REINO" : $"PUNTAJE FINAL  {gm.score:000000}",text);
        if (Scene=="Menu") {
            if(GUI.Button(new Rect(295,245,370,54),(selection==0 ? ">  " : "")+"1 JUGADOR",button)) gm.NewGame();
            if(GUI.Button(new Rect(295,310,370,54),(selection==1 ? ">  " : "")+"COMENZAR AVENTURA",button)) gm.NewGame();
            if(GUI.Button(new Rect(295,375,370,54),(selection==2 ? ">  " : "")+"SELECTOR DE NIVELES",button)) selectingLevel=true;
            GUI.Label(new Rect(50,437,860,35),"A / D o flechas: mover y girar en el aire    ESPACIO: saltar / nadar",new GUIStyle(text){fontSize=17});
            GUI.Label(new Rect(70,468,820,25),"S: entrar en tuberias    M: activar / silenciar musica",new GUIStyle(text){fontSize=15});
        } else {
            if(GUI.Button(new Rect(295,280,370,58),"VOLVER A JUGAR",button)) gm.NewGame();
            if(GUI.Button(new Rect(295,350,370,58),"MENU PRINCIPAL",button)) SceneManager.LoadScene("Menu");
        }
        GUI.Label(new Rect(70,495,820,30),"ENTER PARA COMENZAR",text);
    }

    private void DrawSelector(GameManager gm) {
        GUI.Label(new Rect(0,40,960,85),"SELECTOR DE NIVELES",title);
        GUI.Label(new Rect(0,125,960,40),"PRUEBA CUALQUIER ESCENARIO CON 3 VIDAS",text);
        string[] names={"1-1  SUPERFICIE","1-2  SUBTERRANEO","1-3  ACUATICO","1-4  FORJA DE BOWSER"};
        for(int i=0;i<4;i++)if(GUI.Button(new Rect(105+(i%2)*390,205+(i/2)*95,360,72),names[i],button)){selectingLevel=false;gm.StartTestLevel(i+1);}
        if(GUI.Button(new Rect(330,435,300,52),"VOLVER",button))selectingLevel=false;
    }

    private void DrawMap(GameManager gm) {
        if(mapLandscape==null)mapLandscape=MakeLandscape();
        GUI.DrawTexture(new Rect(0,0,960,540),mapLandscape);
        Panel(new Rect(22,20,916,98),new Color(.035f,.10f,.16f,.94f));
        GUI.Label(new Rect(42,25,620,48),"EL REINO CHAMPINON",new GUIStyle(title){fontSize=34,alignment=TextAnchor.MiddleLeft});
        GUI.Label(new Rect(44,74,610,28),$"VIDAS  {gm.lives}     MONEDAS  {gm.coins:00}     PUNTAJE  {gm.score:000000}",new GUIStyle(text){fontSize=17,alignment=TextAnchor.MiddleLeft});
        string[] names={"PRADERA","SUBSUELO","OCEANO","LA FORJA"};
        Vector2[] points={new Vector2(150,320),new Vector2(355,245),new Vector2(575,330),new Vector2(790,235)};
        for(int i=0;i<3;i++){
            Vector2 d=points[i+1]-points[i];int dots=60;
            for(int j=0;j<dots;j++){float t=j/(dots-1f);var p=points[i]+d*t+new Vector2(0,Mathf.Sin(t*Mathf.PI)*(i==1?25:-20));Panel(new Rect(p.x-7,p.y-7,14,14),new Color(.28f,.23f,.13f));if(j%5<3)Panel(new Rect(p.x-3,p.y-3,6,6),i<gm.unlockedStage-1?new Color(1,.87f,.43f):new Color(.62f,.66f,.52f));}
        }
        GUI.color=Color.white;
        for(int i=0;i<4;i++){
            var p=points[i];bool open=i<gm.unlockedStage;GUI.enabled=open;
            GUI.backgroundColor=open?new Color(1,.82f,.33f):new Color(.4f,.5f,.46f);
            if(GUI.Button(new Rect(p.x-35,p.y-24,70,52),open?"1-"+(i+1):"?",button))gm.LoadLevel(1,i+1);
            GUI.backgroundColor=Color.white;GUI.enabled=true;
            Panel(new Rect(p.x-83,p.y+36,166,29),new Color(.03f,.10f,.13f,.9f));
            GUI.Label(new Rect(p.x-83,p.y+34,166,30),names[i],new GUIStyle(text){fontSize=15,fontStyle=FontStyle.Bold});
            if(i==1&&mapPipe!=null)GUI.DrawTexture(new Rect(p.x-28,p.y-66,56,28),mapPipe,ScaleMode.ScaleToFit,true);
            if(i==3&&mapCastle!=null)GUI.DrawTexture(new Rect(p.x-48,p.y-112,96,88),mapCastle,ScaleMode.ScaleToFit,true);
            if(i==gm.stage-1&&mapPlayer!=null)GUI.DrawTexture(new Rect(p.x-21,p.y-113+Mathf.Sin(Time.unscaledTime*4)*4,42,48),mapPlayer,ScaleMode.ScaleToFit,true);
        }
        Panel(new Rect(22,450,916,68),new Color(.035f,.10f,.16f,.95f));
        GUI.Label(new Rect(42,458,560,42),"ELIGE UN DESTINO  /  ENTER PARA CONTINUAR",new GUIStyle(text){fontSize=16,alignment=TextAnchor.MiddleLeft});
        if(GUI.Button(new Rect(665,461,250,44),"PROBAR NIVELES",button))selectingLevel=true;
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
