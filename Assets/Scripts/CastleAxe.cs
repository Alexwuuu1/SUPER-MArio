using UnityEngine;

public class CastleAxe : MonoBehaviour
{
    public GameObject bridge,boss,exit;
    private bool used;
    private void OnTriggerEnter2D(Collider2D other){
        if(used||!other.CompareTag("Player"))return;used=true;
        Destroy(bridge);Destroy(boss);exit.SetActive(true);MarioAudio.Play("win");GameManager.Instance.AddScore(5000);GetComponent<SpriteRenderer>().enabled=false;
    }
}
