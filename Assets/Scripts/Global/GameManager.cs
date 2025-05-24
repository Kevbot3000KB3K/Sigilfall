using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Central GameManager that handles score, lives, level transitions, and gameplay flow.
/// Implements Singleton pattern and persists across scenes.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private string titleScreenSceneName = "TitleScreen";

    public Ball ball { get; private set; }
    public Paddle paddle { get; private set; }
    public Brick[] bricks { get; private set; }

    public AudioSource lifeLostAudio;

    public int level = 1;
    public int score = 0;
    public int lives = 3;

    private int ballsInPlay = 0;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!IsSceneLoaded(titleScreenSceneName))
            {
                SceneManager.LoadScene(titleScreenSceneName, LoadSceneMode.Additive);
            }
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    /// <summary>
    /// Checks if a scene is currently loaded by name.
    /// </summary>
    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name == sceneName)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Starts a brand new game from Level 1 with reset score/lives.
    /// </summary>
    public void NewGame()
    {
        score = 0;
        lives = 3;
        LoadLevel(1);
    }

    /// <summary>
    /// Loads the specified level scene by number.
    /// </summary>
    private void LoadLevel(int level)
    {
        this.level = level;
        SceneManager.LoadScene("level " + level);
    }

    /// <summary>
    /// Called when a new scene is loaded. Finds important objects in the scene.
    /// </summary>
    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        ball = Object.FindFirstObjectByType<Ball>();
        paddle = Object.FindFirstObjectByType<Paddle>();
        bricks = Object.FindObjectsByType<Brick>(FindObjectsSortMode.None);
    }

    /// <summary>
    /// Resets the level by destroying all balls and spawning a new one. Also resets the paddle.
    /// </summary>
    private void ResetLevel()
    {
        ballsInPlay = 0;

        // Destroy all existing balls
        foreach (Ball b in Object.FindObjectsByType<Ball>(FindObjectsSortMode.None))
        {
            Destroy(b.gameObject);
        }

        // Load ball prefab and spawn a new one
        GameObject ballPrefab = Resources.Load<GameObject>("Prefabs/Ball");
        if (ballPrefab != null)
        {
            GameObject newBallObj = Instantiate(ballPrefab);
            ball = newBallObj.GetComponent<Ball>();
            ball.ResetBall();
        }
        else
        {
            Debug.LogError("Ball prefab not found in Resources/Prefabs!");
        }

        if (paddle != null)
        {
            paddle.ResetPaddle();
        }
    }

    /// <summary>
    /// Called when the player runs out of lives.
    /// </summary>
    private void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    /// <summary>
    /// Called when the ball falls and the player misses it.
    /// </summary>
    public void Miss()
    {
        lives--;

        if (lives > 0)
        {
            StartCoroutine(MissSequence());
        }
        else
        {
            GameOver();
        }
    }

    /// <summary>
    /// Adds an extra life to the player (used by ball effects, etc.).
    /// </summary>
    public void GainLife()
    {
        lives++;
        Debug.Log("Life gained! Total lives: " + lives);
    }

    /// <summary>
    /// Coroutine that handles the delay between losing a life and restarting the level.
    /// </summary>
    private IEnumerator MissSequence()
    {
        if (ball != null)
            ball.gameObject.SetActive(false);

        if (lifeLostAudio != null)
            lifeLostAudio.Play();

        yield return new WaitForSeconds(1.5f);

        ResetLevel();

        if (ball != null)
            ball.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called when a brick is hit. Updates score and checks for win condition.
    /// </summary>
    public void Hit(Brick brick)
    {
        score += brick.points;

        if (Cleared())
        {
            // Destroy remaining balls
            Ball[] allBalls = Object.FindObjectsByType<Ball>(FindObjectsSortMode.None);
            foreach (Ball b in allBalls)
            {
                Destroy(b.gameObject);
            }
            ballsInPlay = 0;

            // Show victory screen
            VictoryManager victoryManager = Object.FindFirstObjectByType<VictoryManager>();
            if (victoryManager != null)
                victoryManager.ShowVictory();
        }
    }

    /// <summary>
    /// Checks if all breakable bricks are destroyed.
    /// </summary>
    private bool Cleared()
    {
        foreach (Brick brick in bricks)
        {
            if (brick == null) continue;

            if (brick.gameObject != null &&
                brick.gameObject.activeInHierarchy &&
                !brick.unbreakable)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Tracks how many balls are in play when spawning a new one.
    /// </summary>
    public void RegisterBall()
    {
        ballsInPlay++;
        Debug.Log("🟢 Ball registered. Total: " + ballsInPlay);
    }

    /// <summary>
    /// Called when a ball is destroyed. Triggers a life loss if no balls are left.
    /// </summary>
    public void OnBallDestroyed()
    {
        ballsInPlay--;
        Debug.Log("🔴 Ball destroyed. Remaining: " + ballsInPlay);

        if (ballsInPlay <= 0)
        {
            Debug.Log("💀 All balls lost. Losing a life.");
            Miss();
        }
    }

    /// <summary>
    /// Optional: Goes back to the title screen manually.
    /// </summary>
    public void GoToTitleScreen()
    {
        SceneManager.LoadScene(titleScreenSceneName);
    }
}
