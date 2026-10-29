using UnityEngine;

public class LevelHazard : MonoBehaviour
{
    public bool instant;
    private void OnTriggerEnter2D(Collider2D other){Touch(other);}
    private void OnTriggerStay2D(Collider2D other){Touch(other);}
    private void Touch(Collider2D other){if(!other.CompareTag("Player"))return;var p=other.GetComponent<Player>();if(p.dead)return;if(instant)p.Death();else p.Hit();}
}
