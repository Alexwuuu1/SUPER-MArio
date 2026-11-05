using UnityEngine;

public class MarioFireball : MonoBehaviour
{
    private static Sprite sprite;
    private Vector2 velocity;
    private Rigidbody2D body;
    private Player owner;
    private bool spent;
    public static MarioFireball Launch(Player owner,int direction){
        var go=new GameObject("Bola de fuego");go.transform.position=owner.transform.position+new Vector3(direction*.7f,.55f,0);
        var sr=go.AddComponent<SpriteRenderer>();
        if(sprite==null){var t=new Texture2D(8,8,TextureFormat.RGBA32,false);t.filterMode=FilterMode.Point;for(int y=0;y<8;y++)for(int x=0;x<8;x++){float d=Vector2.Distance(new Vector2(x,y),new Vector2(3.5f,3.5f));t.SetPixel(x,y,d>3.5f?Color.clear:d<1.8f?new Color(1,1,.5f):new Color(1,.32f,.02f));}t.Apply();sprite=Sprite.Create(t,new Rect(0,0,8,8),new Vector2(.5f,.5f),16);}
        sr.sprite=sprite;sr.sortingOrder=8;
        var hit=go.AddComponent<CircleCollider2D>();hit.radius=.21f;
        var rb=go.AddComponent<Rigidbody2D>();rb.gravityScale=0;rb.freezeRotation=true;rb.collisionDetectionMode=CollisionDetectionMode2D.Continuous;
        foreach(var other in owner.GetComponentsInChildren<Collider2D>())Physics2D.IgnoreCollision(hit,other);
        var ball=go.AddComponent<MarioFireball>();ball.body=rb;ball.owner=owner;ball.velocity=new Vector2(direction*10,-2);
        Destroy(go,3);return ball;
    }
    private void FixedUpdate(){if(spent)return;velocity.y=Mathf.Max(-12,velocity.y-24*Time.fixedDeltaTime);body.linearVelocity=velocity;}
    private void Update(){transform.Rotate(0,0,-540*Time.deltaTime);}
    private void OnCollisionEnter2D(Collision2D hit){
        if(spent)return;
        var enemy=hit.gameObject.GetComponent<Goomba>();var koopa=hit.gameObject.GetComponent<Koopa>();
        if(enemy!=null||koopa!=null){ImpactEnemy(enemy,koopa);return;}
        if(hit.gameObject.layer!=LayerMask.NameToLayer("Default")){Spend();return;}
        for(int i=0;i<hit.contactCount;i++)if(hit.GetContact(i).normal.y>.6f){velocity.y=7;return;}
        Spend();
    }
    private void OnTriggerEnter2D(Collider2D hit){
        if(hit.GetComponentInParent<Player>()==owner)return;
        var enemy=hit.GetComponent<Goomba>();var koopa=hit.GetComponent<Koopa>();
        if(enemy!=null||koopa!=null)ImpactEnemy(enemy,koopa);
    }
    private void ImpactEnemy(Goomba goomba,Koopa koopa){if(spent)return;if(goomba!=null)goomba.HitByFire();if(koopa!=null)koopa.HitByFire();MarioAudio.Play("impact");Spend();}
    private void Spend(){if(spent)return;spent=true;Destroy(gameObject);}
}
