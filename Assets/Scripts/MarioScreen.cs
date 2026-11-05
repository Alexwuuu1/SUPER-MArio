using UnityEngine;
using UnityEngine.SceneManagement;

public class MarioScreen : MonoBehaviour
{
    private GUIStyle title, text, button, hud;
    private int selection;
    private bool selectingLevel;
    public Texture2D mapPlayer, mapCastle, mapPipe;
    private MarioWorldMap worldMap;
    private string Scene => SceneManager.GetActiveScene().name;
    private void Update()
    {
        if(MarioPauseMenu.IsPaused)return;
        if (selectingLevel) { if(Input.GetKeyDown(KeyCode.Escape))selectingLevel=false;return; }
        if (Scene == "Menu") {
            if (Input.GetKeyDown(KeyCode.DownArrow)) { selection = (selection+1)%2; MarioAudio.Play("coin"); }
            if (Input.GetKeyDown(KeyCode.UpArrow)) { selection = (selection+1)%2; MarioAudio.Play("coin"); }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) { if(selection==1)selectingLevel=true;else GameManager.Instance.NewGame(); }
        } else if (Scene=="Map") {
            if (Input.GetKeyDown(KeyCode.Return)) GameManager.Instance.LoadLevel(1,GameManager.Instance.stage);
        } else if (!GameManager.Instance.playing && Input.GetKeyDown(KeyCode.Return)) GameManager.Instance.NewGame();
    }
    private void OnGUI()
    {
        float scale = Mathf.Min(Screen.width / 960f, Screen.height / 540f);
        GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width-960*scale)/2,(Screen.height-540*scale)/2,0), Quaternion.identity, Vector3.one*scale);
        if (title == null) {
            title = new GUIStyle(GUI.skin.label) { fontSize=48, fontStyle=FontStyle.Bold, alignment=TextAnchor.MiddleCenter };
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
        if (Scene=="Map") { if(worldMap==null){worldMap=gameObject.GetComponent<MarioWorldMap>();if(worldMap==null)worldMap=gameObject.AddComponent<MarioWorldMap>();worldMap.mapPlayer=mapPlayer;worldMap.mapCastle=mapCastle;worldMap.mapPipe=mapPipe;}worldMap.Draw(gm,title,text,button,()=>selectingLevel=true); return; }
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
        if(Scene=="Menu"){
            GUI.color=new Color(.015f,.04f,.09f,.82f);GUI.DrawTexture(new Rect(190,100,580,320),Texture2D.whiteTexture);GUI.color=Color.white;
            if(mapPlayer!=null)GUI.DrawTexture(new Rect(80,285,78,95),mapPlayer,ScaleMode.ScaleToFit,true);
            if(mapCastle!=null)GUI.DrawTexture(new Rect(790,260,110,120),mapCastle,ScaleMode.ScaleToFit,true);
        }
        GUI.Label(new Rect(60,90,840,100),Scene=="Menu" ? "SUPER MARIO" : Scene=="GameOver" ? "GAME OVER" : "REINO SALVADO",title);
        GUI.Label(new Rect(80,190,800,45),Scene=="Menu" ? "UNA AVENTURA A TRAVES DEL REINO" : $"PUNTAJE FINAL  {gm.score:000000}",text);
        if (Scene=="Menu") {
            if(GUI.Button(new Rect(265,250,430,62),(selection==0 ? ">  " : "")+"JUGAR AVENTURA",button)) gm.NewGame();
            if(GUI.Button(new Rect(265,330,430,62),(selection==1 ? ">  " : "")+"SELECTOR DE NIVELES",button)) selectingLevel=true;
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

}
