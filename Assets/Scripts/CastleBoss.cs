using UnityEngine;

public class CastleBoss : MonoBehaviour
{
    public GameObject flamePrefab;
    private Vector3 origin;
    private float nextFire=2;
    private void Start(){origin=transform.position;}
    private void Update(){
        transform.position=origin+new Vector3(Mathf.Sin(Time.time)*2,Mathf.Abs(Mathf.Sin(Time.time*.9f))*1.3f,0);
        if(Time.timeSinceLevelLoad<nextFire)return;nextFire=Time.timeSinceLevelLoad+2.4f;
        var fire=Instantiate(flamePrefab,transform.position+Vector3.left*1.5f,Quaternion.identity);fire.SetActive(true);fire.AddComponent<BossFlame>();Destroy(fire,6);
    }
}
public class BossFlame : MonoBehaviour
{
    private void Update(){transform.position+=Vector3.left*5*Time.deltaTime;}
}
