using UnityEngine;

public class LevelMotion : MonoBehaviour
{
    public Vector3 travel=new Vector3(0,3,0);
    public float speed=1;
    public bool rotate;
    private Vector3 origin;
    private void Start(){origin=transform.position;}
    private void Update(){if(rotate)transform.Rotate(0,0,speed*Time.deltaTime);else transform.position=origin+travel*Mathf.Sin(Time.time*speed);}
}
