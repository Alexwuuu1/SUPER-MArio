using UnityEngine;

public class Goomba : MonoBehaviour
{
    private bool defeated;
    public Sprite flatSprite;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player.starpower) {
                Hit();
            } else if (collision.transform.DotTest(transform, Vector2.down)) {
                Flatten();
            } else {
                player.Hit();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shell")) {
            Hit();
        }
    }

    private void Flatten()
    {
        if(defeated)return;defeated=true;
        GameManager.Instance.AddScore(100);
        MarioAudio.Play("stomp");
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EntityMovement>().enabled = false;
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = flatSprite;
        Destroy(gameObject, 0.5f);
    }

    public void HitByFire() { Hit(); }

    private void Hit()
    {
        if (defeated || GetComponent<DeathAnimation>().enabled) return;
        defeated=true;
        GameManager.Instance.AddScore(100);
        MarioAudio.Play("stomp");
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<DeathAnimation>().enabled = true;
        Destroy(gameObject, 3f);
    }

}
