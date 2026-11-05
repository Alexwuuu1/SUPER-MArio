using UnityEngine;
using UnityEngine.SceneManagement;
public class MarioLevelTransition : MonoBehaviour
{
    private float entered;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install(){var go=new GameObject("Level presentation");DontDestroyOnLoad(go);go.AddComponent<MarioLevelTransition>();}
    private void Awake(){SceneManager.sceneLoaded+=Loaded;}
    private void OnDestroy(){SceneManager.sceneLoaded-=Loaded;}
    private void Loaded(Scene scene,LoadSceneMode mode){entered=Time.unscaledTime;}
    private void OnGUI(){
        var gm=GameManager.Instance;if(gm==null||!gm.playing)return;
        float elapsed=Time.unscaledTime-entered;if(elapsed>1.7f)return;
        float alpha=Mathf.Clamp01(1-(elapsed-.9f)/.8f);
        GUI.depth=-10;GUI.color=new Color(.02f,.05f,.1f,.65f*alpha);GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Texture2D.whiteTexture);
        GUI.color=new Color(1,.85f,.4f,alpha);
        string[] names={"PRADERA DEL REINO","GALERIAS SUBTERRANEAS","PROFUNDIDADES DEL OCEANO","LA FORJA DE BOWSER"};
        GUI.Label(new Rect(0,Screen.height*.4f,Screen.width,120),"MUNDO 1-"+gm.stage+"\n"+names[Mathf.Clamp(gm.stage-1,0,3)],new GUIStyle(GUI.skin.label){fontSize=Mathf.Max(18,Screen.height/25),fontStyle=FontStyle.Bold,alignment=TextAnchor.MiddleCenter});GUI.color=Color.white;
    }
}
