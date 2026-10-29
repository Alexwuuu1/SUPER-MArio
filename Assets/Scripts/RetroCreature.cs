using UnityEngine;

public class RetroCreature : MonoBehaviour
{
    public enum Kind { Coral, Blooper, Piranha }
    public Kind kind;
    private Vector3 origin;
    private Player player;
    private SpriteRenderer renderer2D;
    private void Start(){
        origin=transform.position;player=FindFirstObjectByType<Player>();renderer2D=GetComponent<SpriteRenderer>();
        string[] pixels=kind==Kind.Blooper?new[]{"....WWWW....","...WWWWWW...","..WWWWWWWW..",".WWWWWWWWWW.","WWWWWWWWWWWW","WWWKKWWKKWWW","WWWKKWWKKWWW",".WWWWWWWWWW.","..WWWWWWWW..",".WW.WW.WW.W.",".W..WW.WW.W.","W...W..W...W"}:kind==Kind.Coral?new[]{"..R.....R...","..R..R..R...","R.R..R..R.R.","R.R..R..R.R.","RRR..R..RRR.","..R..R..R...","..RR.R.RR...","...R.R.R....","...RRRRR....",".....R......",".....R......",".....R......"}:new[]{"..GGGGGGGG..",".GGWGGGWGGG.","GGGGGGGGGGGG","WWWW....WWWW","...WW..WW...","WWWW....WWWW","GGGGGGGGGGGG",".GGWGGGWGGG.","..GGGGGGGG..",".....GG.....",".GG..GG..GG.","..GGGGGGGG.."};
        var texture=new Texture2D(12,12,TextureFormat.RGBA32,false);texture.filterMode=FilterMode.Point;
        for(int y=0;y<12;y++)for(int x=0;x<12;x++){char c=pixels[y][x];texture.SetPixel(x,11-y,c=='W'?new Color(1,.96f,.85f):c=='K'?Color.black:c=='G'?new Color(.2f,.8f,.15f):c=='R'?new Color(1,.25f,.5f):Color.clear);}
        texture.Apply();renderer2D.sprite=Sprite.Create(texture,new Rect(0,0,12,12),new Vector2(.5f,.5f),12);renderer2D.color=Color.white;
    }
    private void Update(){
        if(kind==Kind.Coral)return;
        if(kind==Kind.Piranha){float cycle=Mathf.Sin(Time.time*1.3f+origin.x);bool near=player!=null&&Mathf.Abs(player.transform.position.x-origin.x)<2.2f;float rise=near?0:Mathf.Clamp01(cycle*2);transform.position=origin+Vector3.up*rise*1.6f;renderer2D.enabled=rise>.2f;GetComponent<Collider2D>().enabled=rise>.5f;}
        else {Vector3 toward=player==null?Vector3.zero:player.transform.position-origin;toward.x=Mathf.Clamp(toward.x,-3,3);transform.position=origin+new Vector3(toward.x*.25f+Mathf.Sin(Time.time*.8f)*1.6f,Mathf.Sin(Time.time*1.6f)*1.8f,0);}
    }
}
