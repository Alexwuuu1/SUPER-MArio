using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SideScrolling : MonoBehaviour
{
    private new Camera camera;
    private Transform player;
    private float leftLimit, rightLimit, followVelocity;
    private float lookAhead=3, lastPlayerX;

    public float height = 6.5f;
    public float undergroundHeight = -9.5f;
    public float undergroundThreshold = 0f;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        player = FindFirstObjectByType<Player>().transform;
        lastPlayerX=player.position.x;
        leftLimit = transform.position.x;
        float edge = leftLimit + 20;
        foreach (var sprite in FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)) edge = Mathf.Max(edge, sprite.bounds.max.x);
        rightLimit = edge - camera.orthographicSize * camera.aspect;
    }

    private void LateUpdate()
    {
        if(player==null || Time.timeScale==0)return;
        float dx=player.position.x-lastPlayerX;lastPlayerX=player.position.x;
        if(Mathf.Abs(dx)>.005f)lookAhead=Mathf.MoveTowards(lookAhead,Mathf.Sign(dx)*3,Time.deltaTime*8);
        // track the player moving to the right
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.SmoothDamp(cameraPosition.x, Mathf.Clamp(player.position.x + lookAhead, leftLimit, Mathf.Max(leftLimit,rightLimit)), ref followVelocity, .12f);
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
