using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    public bool finalExit;
    private bool entered;
    private void OnTriggerEnter2D(Collider2D other) {
        if(entered || !other.CompareTag("Player") || other.GetComponent<Player>().dead)return;
        entered=true;
        MarioAudio.Play("win");
        GameManager.Instance.AddScore(1000);
        GameManager.Instance.NextLevel();
    }
}
