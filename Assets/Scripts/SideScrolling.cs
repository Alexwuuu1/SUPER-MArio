using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SideScrolling : MonoBehaviour
{
    private new Camera camera;
    private Transform player;
    private float leftLimit, rightLimit, followVelocity;

    public float height = 6.5f;
    public float undergroundHeight = -9.5f;
    public float undergroundThreshold = 0f;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        player = FindFirstObjectByType<Player>().transform;
        leftLimit = transform.position.x;
        float edge = leftLimit + 20;
        foreach (var sprite in FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)) edge = Mathf.Max(edge, sprite.bounds.max.x);
        rightLimit = edge - camera.orthographicSize * camera.aspect;
    }

    private void LateUpdate()
    {
        // track the player moving to the right
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.SmoothDamp(cameraPosition.x, Mathf.Clamp(player.position.x + 3, leftLimit, rightLimit), ref followVelocity, .12f);
        transform.position = cameraPosition;
    }

    public void SetUnderground(bool underground)
    {
        // set underground height offset
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;
    }

}
