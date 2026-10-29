using UnityEngine;
using UnityEngine.SceneManagement;

public class MarioScreen : MonoBehaviour
{
    private GUIStyle title, text, button, hud;
    private int selection;
    private string Scene => SceneManager.GetActiveScene().name;
    private void Update()
    {
        if (Scene == "Menu") {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow)) { selection = 1-selection; MarioAudio.Play("coin"); }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) GameManager.Instance.NewGame();
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
        if (gm.playing) {
            GUI.color=new Color(0,0,0,.8f); GUI.DrawTexture(new Rect(0,0,960,58),Texture2D.whiteTexture); GUI.color=Color.white;
            GUI.Label(new Rect(28,10,250,45),$"MARIO   {gm.score:000000}",hud);
            GUI.Label(new Rect(300,10,170,45),$"COINS  {gm.coins:00}",hud);
            GUI.Label(new Rect(500,10,200,45),$"WORLD  {gm.world}-{gm.stage}",hud);
            GUI.Label(new Rect(730,10,220,45),$"TIME {(int)gm.timeRemaining:000}  x{gm.lives}",hud);
            if (Time.timeSinceLevelLoad < 3) GUI.Label(new Rect(0,80,960,35),gm.stage==2 ? "1-2  /  SUBTERRANEO" : gm.stage==3 ? "1-3  /  PROFUNDIDADES  -  ESPACIO PARA NADAR" : gm.stage==4 ? "1-4  /  LA FORJA DE BOWSER" : "1-1  /  REINO CHAMPINON",text);
            return;
        }
        GUI.Label(new Rect(60,90,840,100),Scene=="Menu" ? "SUPER MARIO" : Scene=="GameOver" ? "GAME OVER" : "REINO SALVADO",title);
        GUI.Label(new Rect(80,190,800,45),Scene=="Menu" ? "UNA AVENTURA A TRAVES DEL REINO" : $"PUNTAJE FINAL  {gm.score:000000}",text);
        if (Scene=="Menu") {
            if(GUI.Button(new Rect(295,275,370,58),(selection==0 ? ">  " : "")+"1 JUGADOR",button)) gm.NewGame();
            if(GUI.Button(new Rect(295,347,370,58),(selection==1 ? ">  " : "")+"COMENZAR AVENTURA",button)) gm.NewGame();
            GUI.Label(new Rect(70,440,820,40),"A / D o flechas: mover     ESPACIO: saltar / nadar",text);
        } else {
            if(GUI.Button(new Rect(295,280,370,58),"VOLVER A JUGAR",button)) gm.NewGame();
            if(GUI.Button(new Rect(295,350,370,58),"MENU PRINCIPAL",button)) SceneManager.LoadScene("Menu");
        }
        GUI.Label(new Rect(70,495,820,30),"ENTER PARA COMENZAR",text);
    }
}
