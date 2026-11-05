using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    public float end=110;
    private Transform target;
    private float speed;
    [Range(.03f,.5f)] public float smoothing=.14f;
    public float lookAhead=2.5f;
    private float ahead;
    private float lastX;
    private void Start(){var p=FindFirstObjectByType<Player>();if(p!=null){target=p.transform;lastX=target.position.x;}}
    private void LateUpdate(){
        if(target==null)return;
        if(Time.timeScale==0)return;
        float half=GetComponent<Camera>().orthographicSize*GetComponent<Camera>().aspect;
        float dx=target.position.x-lastX;lastX=target.position.x;
        if(Mathf.Abs(dx)>.005f)ahead=Mathf.MoveTowards(ahead,Mathf.Sign(dx)*lookAhead,Time.deltaTime*8);
        float x=Mathf.Clamp(target.position.x+ahead,half,Mathf.Max(half,end-half));
        var p=transform.position;p.x=Mathf.SmoothDamp(p.x,x,ref speed,smoothing);transform.position=p;
    }
}
