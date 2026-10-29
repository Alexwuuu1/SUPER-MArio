using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int world { get; private set; }
    public int stage { get; private set; }
    public int lives { get; private set; }
    public int coins { get; private set; }
    public int score { get; private set; }
    public float timeRemaining { get; private set; } = 400f;
    public bool playing => SceneManager.GetActiveScene().name.StartsWith("1-");

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        if (playing) { lives = 3; world = 1; stage = 1; }
    }

    public void NewGame()
    {
        lives = 3;
        coins = 0;
        score = 0;

        LoadLevel(1, 1);
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void LoadLevel(int world, int stage)
    {
        this.world = world;
        this.stage = stage;
        timeRemaining = 400f;

        SceneManager.LoadScene($"{world}-{stage}");
    }

    public void NextLevel()
    {
        if (stage >= 4) SceneManager.LoadScene("Victory");
        else LoadLevel(world, stage + 1);
    }

    public void ResetLevel(float delay)
    {
        CancelInvoke(nameof(ResetLevel));
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel()
    {
        lives--;

        if (lives > 0) {
            LoadLevel(world, stage);
        } else {
            GameOver();
        }
    }

    public void AddCoin()
    {
        coins++;
        AddScore(200);
        MarioAudio.Play("coin");

        if (coins == 100)
        {
            coins = 0;
            AddLife();
        }
    }

    public void AddLife()
    {
        lives++;
    }

    public void AddScore(int points) { score += points; }

    private void Update()
    {
        if (!playing) return;
        var player = FindFirstObjectByType<Player>();
        if (player == null || player.dead || (!player.GetComponent<PlayerMovement>().enabled && player.GetComponent<WaterMotion>() == null)) return;
        timeRemaining = Mathf.Max(0, timeRemaining - Time.deltaTime);
        if (timeRemaining <= 0) player.Death();
    }

}
