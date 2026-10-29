using UnityEngine;

public class LavaSurface : MonoBehaviour
{
    private SpriteRenderer surface;
    private void Start(){
        surface=GetComponent<SpriteRenderer>();Vector2 size=surface.bounds.size;
        var texture=new Texture2D(32,8,TextureFormat.RGBA32,false);texture.filterMode=FilterMode.Point;
        for(int y=0;y<8;y++)for(int x=0;x<32;x++){float wave=Mathf.Sin(x*.65f);texture.SetPixel(x,y,y>5+wave?new Color(1,.85f,.2f):y>3?new Color(1,.35f,.03f):new Color(.85f,.07f,.01f));}
        texture.Apply();surface.sprite=Sprite.Create(texture,new Rect(0,0,32,8),new Vector2(.5f,.5f),8,0,SpriteMeshType.FullRect);
        transform.localScale=Vector3.one;surface.drawMode=SpriteDrawMode.Tiled;surface.size=size;surface.color=Color.white;
        var hit=GetComponent<BoxCollider2D>();if(hit!=null)hit.size=size;
    }
    private void Update(){if(surface!=null)surface.color=Color.Lerp(Color.white,new Color(1,.65f,.6f),(.5f+.5f*Mathf.Sin(Time.time*4))*.3f);}
}
