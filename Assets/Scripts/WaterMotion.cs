using UnityEngine;

public class WaterMotion : MonoBehaviour
{
    private PlayerMovement movement;
    private Rigidbody2D body;
    private float vy;
    private bool queued;
    private void Start(){movement=GetComponent<PlayerMovement>();movement.enabled=false;body=GetComponent<Rigidbody2D>();body.bodyType=RigidbodyType2D.Dynamic;GetComponent<Collider2D>().enabled=true;}
    private void Update(){
        if(GetComponent<Player>().dead)return;
        if(Input.GetButtonDown("Jump")){queued=true;MarioAudio.Play("jump");}
        float axis=Input.GetAxisRaw("Horizontal");if(axis!=0)transform.eulerAngles=new Vector3(0,axis<0?180:0,0);
    }
    private void FixedUpdate(){
        if(GetComponent<Player>().dead)return;
        if(queued){vy=5.5f;queued=false;}
        vy=Mathf.Max(vy-8*Time.fixedDeltaTime,-2.7f);
        var p=body.position+new Vector2(Input.GetAxisRaw("Horizontal")*4,vy)*Time.fixedDeltaTime;
        p.y=Mathf.Clamp(p.y,1.1f,10.5f);p.x=Mathf.Clamp(p.x,.7f,159);
        body.MovePosition(p);
    }
}
