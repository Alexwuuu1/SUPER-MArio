using UnityEngine;

public class LevelLift : MonoBehaviour
{
    public float speed=1.7f;
    public bool descending;
    private float x;
    private float lastY;
    private float offset;
    private Player rider;
    private void Start(){x=transform.position.x;lastY=transform.position.y;offset=descending?11-lastY:lastY+1;}
    private void FixedUpdate(){
        float phase=Mathf.Repeat(Time.time*speed+offset,12);
        float y=descending?11-phase:phase-1;
        transform.position=new Vector3(x,y,0);
        float delta=y-lastY;
        if(rider!=null&&!rider.dead&&Mathf.Abs(delta)<1){var rb=rider.GetComponent<Rigidbody2D>();rb.position+=Vector2.up*delta;}
        lastY=y;
    }
    private void OnCollisionStay2D(Collision2D hit){if(hit.gameObject.CompareTag("Player")&&hit.transform.position.y>transform.position.y+.35f)rider=hit.gameObject.GetComponent<Player>();}
    private void OnCollisionExit2D(Collision2D hit){if(hit.gameObject.CompareTag("Player"))rider=null;}
}
