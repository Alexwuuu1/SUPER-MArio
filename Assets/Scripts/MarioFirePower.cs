using UnityEngine;
using UnityEngine.SceneManagement;

public class MarioFirePower : MonoBehaviour
{
    public bool active { get; private set; }
    private Player player;
    private float nextShot;
    private readonly System.Collections.Generic.List<MarioFireball> balls=new System.Collections.Generic.List<MarioFireball>();
    private void Awake(){player=GetComponent<Player>();}
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install(){SceneManager.sceneLoaded-=Loaded;SceneManager.sceneLoaded+=Loaded;Loaded(SceneManager.GetActiveScene(),LoadSceneMode.Single);}
    private static void Loaded(Scene scene,LoadSceneMode mode){
        if(!scene.name.StartsWith("1-"))return;
        var player=FindFirstObjectByType<Player>();if(player!=null&&player.GetComponent<MarioFirePower>()==null)player.gameObject.AddComponent<MarioFirePower>();
        // Upgrade two reachable item blocks per level; keep their existing bump animation.
        var blocks=FindObjectsByType<BlockHit>(FindObjectsSortMode.None);
        System.Array.Sort(blocks,(a,b)=>a.transform.position.x.CompareTo(b.transform.position.x));
        int added=0;
        foreach(var block in blocks){
            if(block.item==null||block.transform.position.y<0||block.transform.position.y>8)continue;
            var power=block.item.GetComponent<PowerUp>();
            if(power==null||power.type!=PowerUp.Type.MagicMushroom)continue;
            var flower=CreateFlower();flower.SetActive(false);block.item=flower;added++;if(added==2)break;
        }
        if(added==0&&player!=null){var flower=CreateFlower();flower.transform.position=player.transform.position+Vector3.right*3;}
    }
    public static GameObject CreateFlower(){
        var go=new GameObject("Flor de fuego");var sr=go.AddComponent<SpriteRenderer>();sr.sprite=FireFlowerPickup.FlowerSprite;sr.sortingOrder=4;
        go.AddComponent<BoxCollider2D>().isTrigger=true;go.AddComponent<FireFlowerPickup>();return go;
    }
    public void Grant(){if(player.dead)return;if(!player.big)player.Grow();active=true;MarioAudio.Play("powerup");}
    public bool TryShoot(int direction){
        balls.RemoveAll(ball=>ball==null);
        if(!active||player.dead||Time.timeScale==0||(!player.GetComponent<PlayerMovement>().enabled&&player.GetComponent<WaterMotion>()==null)||Time.time<nextShot||balls.Count>=2)return false;
        balls.Add(MarioFireball.Launch(player,direction<0?-1:1));nextShot=Time.time+.25f;MarioAudio.Play("fire");return true;
    }
    private void Update(){if(Input.GetKeyDown(KeyCode.X))TryShoot(transform.eulerAngles.y>90?-1:1);}
    private void LateUpdate(){
        if(!active)return;
        if(player.dead||player.capsuleCollider.size.y<1.5f){Lose();return;}
        if(!player.starpower)player.bigRenderer.spriteRenderer.color=new Color(1,.65f,.35f);
    }
    public void Lose(){active=false;if(player!=null&&player.bigRenderer.spriteRenderer!=null)player.bigRenderer.spriteRenderer.color=Color.white;}
    private void OnDestroy(){Lose();}
}
public class FireFlowerPickup : MonoBehaviour
{
    private static Sprite sprite;
    public static Sprite FlowerSprite {
        get {
            if(sprite!=null)return sprite;
            string[] rows={"................",".....RRRRRR.....","...RRYYYYYYRR...","..RYYWWWWWWYYR..","..RYWWKKKKWWYR..","..RYWWKKKKWWYR..","..RYYWWWWWWYYR..","...RRYYYYYYRR...",".....RRRRRR.....",".......GG.......","..GG...GG...GG..","..GGGG.GG.GGGG..","...GGGGGGGGGG...",".....GGGGGG.....",".......GG.......","................"};
            var t=new Texture2D(16,16,TextureFormat.RGBA32,false);t.filterMode=FilterMode.Point;
            for(int y=0;y<16;y++)for(int x=0;x<16;x++){char c=rows[y][x];t.SetPixel(x,15-y,c=='R'?new Color(1,.2f,.08f):c=='Y'?new Color(1,.75f,.1f):c=='W'?Color.white:c=='K'?Color.black:c=='G'?new Color(.2f,.75f,.2f):Color.clear);}
            t.Apply();sprite=Sprite.Create(t,new Rect(0,0,16,16),new Vector2(.5f,.5f),16);return sprite;
        }
    }
    private float born;
    private Vector3 origin;
    private bool collected;
    private void Awake(){born=Time.time;origin=transform.position;}
    private void Start(){born=Time.time;origin=transform.position;}
    private void OnEnable(){born=Time.time;origin=transform.position;}
    private void Update(){transform.position=origin+Vector3.up*Mathf.SmoothStep(0,1,Mathf.Clamp01((Time.time-born)/.45f));}
    private void OnTriggerEnter2D(Collider2D other){Collect(other);}
    private void OnTriggerStay2D(Collider2D other){Collect(other);}
    private void Collect(Collider2D other){
        if(collected||Time.time-born<.45f)return;
        var p=other.GetComponentInParent<Player>();if(p==null||p.dead)return;
        var power=p.GetComponent<MarioFirePower>();if(power==null)power=p.gameObject.AddComponent<MarioFirePower>();
        collected=true;power.Grant();GameManager.Instance.AddScore(1000);Destroy(gameObject);
    }
}
