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
        ball.ResetBall();
        paddle.ResetPaddle();
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
            if (brick.gameObject.activeInHierarchy && !brick.unbreakable)
                return false;
        }
        return true;
    }
}
