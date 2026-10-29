using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    public float end=110;
    private Transform target;
    private float speed;
    private void Start(){target=FindFirstObjectByType<Player>().transform;}
    private void LateUpdate(){
        if(target==null)return;
        float half=GetComponent<Camera>().orthographicSize*GetComponent<Camera>().aspect;
        float x=Mathf.Clamp(target.position.x+3,half,end-half);
        var p=transform.position;p.x=Mathf.SmoothDamp(p.x,x,ref speed,.16f);transform.position=p;
    }
}
