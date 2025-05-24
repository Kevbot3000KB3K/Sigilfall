using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private string titleScreenSceneName = "TitleScreen";
    public Ball ball { get; private set; }
    public AudioSource lifeLostAudio;

    public Paddle paddle { get; private set; }
    public Brick[] bricks { get; private set; }
    public GameObject startImage;

    public int level = 1;
    public int score = 0;
    public int lives = 3;
    private int ballsInPlay = 0;

    private void Awake()
    {
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

    public void NewGame()
    {
        score = 0;
        lives = 3;
        LoadLevel(1);
    }

    private void LoadLevel(int level)
    {
        this.level = level;
        SceneManager.LoadScene("level " + level);
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (ball == null)
            ball = FindObjectOfType<Ball>();

        if (paddle == null)
            paddle = FindObjectOfType<Paddle>();

        if (bricks == null || bricks.Length == 0)
            bricks = FindObjectsOfType<Brick>();
    }

    private void ResetLevel()
    {
        ballsInPlay = 0;

        // Destroy all remaining balls
        foreach (Ball b in FindObjectsOfType<Ball>())
        {
            Destroy(b.gameObject);
        }

        // Instantiate a new ball
        GameObject ballPrefab = Resources.Load<GameObject>("Prefabs/Ball"); // adjust path!
        if (ballPrefab != null)
        {
            GameObject newBallObj = Instantiate(ballPrefab);
            ball = newBallObj.GetComponent<Ball>();
            ball.ResetBall(); // make it follow the paddle again
        }
        else
        {
            Debug.LogError("Ball prefab not found in Resources/Prefabs!");
        }

        // Reset paddle
        if (paddle != null)
        {
            paddle.ResetPaddle();
        }
    }




    private void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

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

    public void GainLife()
    {
        lives++;
        Debug.Log("Life gained! Total lives: " + lives);
        // TODO: update UI if needed
    }

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

    public void Hit(Brick brick)
    {
        score += brick.points;

        if (Cleared())
        {
            VictoryManager victoryManager = FindObjectOfType<VictoryManager>();
            if (victoryManager != null)
                victoryManager.ShowVictory();
        }
    }

    private bool Cleared()
    {
        foreach (Brick brick in bricks)
        {
            if (brick == null) continue; // ✅ Check if reference is missing

            // ✅ Safe access of GameObject
            if (brick.gameObject != null && brick.gameObject.activeInHierarchy && !brick.unbreakable)
                return false;
        }
        return true;

    // ✅ Destroy all balls
    Ball[] allBalls = FindObjectsOfType<Ball>();
        foreach (Ball b in allBalls)
        {
            Destroy(b.gameObject);
        }

        // ✅ Reset tracking
        ballsInPlay = 0;

        return true;
    }

    public void RegisterBall()
    {
        ballsInPlay++;
        Debug.Log("🟢 Ball registered. Total: " + ballsInPlay);
    }

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

}
