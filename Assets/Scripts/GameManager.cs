using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureSession()
    {
        if (Instance == null) new GameObject("Game Manager").AddComponent<GameManager>();
        if (Instance.GetComponent<MarioScreen>() == null) Instance.gameObject.AddComponent<MarioScreen>();
        if (Instance.GetComponent<MarioAudio>() == null) Instance.gameObject.AddComponent<MarioAudio>();
    }
    public static GameManager Instance { get; private set; }

    public int world { get; private set; }
    public int stage { get; private set; }
    public int lives { get; private set; }
    public int coins { get; private set; }
    public int score { get; private set; }
    public int unlockedStage { get; private set; } = 1;
    public bool testingLevel { get; private set; }
    public float timeRemaining { get; private set; } = 400f;
    public bool playing => SceneManager.GetActiveScene().name.StartsWith("1-");
    private bool changingScene;
    private void SceneReady(Scene scene,LoadSceneMode mode) { changingScene=false; }

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneReady;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            SceneManager.sceneLoaded -= SceneReady;
            Instance = null;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        if (playing) {
            lives = 3; world = 1;
            var name = SceneManager.GetActiveScene().name;
            stage = int.Parse(name.Substring(2));
            unlockedStage = stage;
        }
    }

    public void NewGame()
    {
        CancelInvoke(); changingScene=true;
        lives = 3;
        coins = 0;
        score = 0;
        unlockedStage = 1;
        testingLevel = false;
        stage = 1;
        SceneManager.LoadScene("Map");
    }

    public void StartTestLevel(int selectedStage)
    {
        CancelInvoke();
        lives = 3; coins = 0; score = 0; unlockedStage = 4; testingLevel = true;
        LoadLevel(1,Mathf.Clamp(selectedStage,1,4));
    }

    public void GameOver()
    {
        CancelInvoke(); changingScene=true;
        SceneManager.LoadScene("GameOver");
    }

    public void LoadLevel(int world, int stage)
    {
        CancelInvoke(); changingScene=true;
        this.world = world;
        this.stage = stage;
        timeRemaining = 400f;

        SceneManager.LoadScene($"{world}-{stage}");
    }

    public void NextLevel()
    {
        CancelInvoke(); changingScene=true;
        if (stage >= 4) SceneManager.LoadScene("Victory");
        else { unlockedStage = Mathf.Max(unlockedStage,stage+1); stage++; SceneManager.LoadScene("Map"); }
    }

    public void ResetLevel(float delay)
    {
        if(changingScene || !playing)return;
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
