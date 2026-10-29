using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteRenderer : MonoBehaviour
{
    private PlayerMovement movement;
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite idle;
    public Sprite jump;
    public Sprite slide;
    public AnimatedSprite run;

    private void Awake()
    {
        movement = GetComponentInParent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        if (movement.GetComponent<WaterMotion>() != null) {
            run.enabled = false;
            spriteRenderer.sprite = Mathf.Sin(Time.time * 10f) > 0 ? jump : idle;
            return;
        }
        run.enabled = movement.running;

        if (movement.jumping) {
            spriteRenderer.sprite = jump;
        } else if (movement.sliding) {
            spriteRenderer.sprite = slide;
        } else if (!movement.running) {
            spriteRenderer.sprite = idle;
        }
    }

    private void OnEnable()
    {
        spriteRenderer.enabled = true;
    }

    private void OnDisable()
    {
        spriteRenderer.enabled = false;
        run.enabled = false;
    }

}
