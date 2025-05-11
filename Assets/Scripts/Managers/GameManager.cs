using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } 
    [SerializeField] private string titleScreenSceneName = "TitleScreen";
    public Ball ball { get; private set; }
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

    private void Start()
    {
        
    }

    public void NewGame()
    {
        this.score = 0;
        this.lives = 3;
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
        {
            ball = FindObjectOfType<Ball>(); 
        }
        if (paddle == null)
        {
            paddle = FindObjectOfType<Paddle>(); 
        }
        if (bricks == null || bricks.Length == 0)
        {
            bricks = FindObjectsOfType<Brick>(); 
        }
    }

    private void ResetLevel()
    {
        this.ball.ResetBall();
        this.paddle.ResetPaddle();
    }

    private void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void Miss()
    {
        this.lives--;

        if (this.lives > 0)
        {
            ResetLevel();
        }
        else
        {
            GameOver();
        }
    }

    public void Hit(Brick brick)
    {
        this.score += brick.points;

        if (Cleared())
        {
            VictoryManager victoryManager = FindObjectOfType<VictoryManager>();
            if (victoryManager != null)
            {
                victoryManager.ShowVictory();
            }
        }
    }


    private bool Cleared()
    {
        for (int i = 0; i < this.bricks.Length; i++)
        {
            if (this.bricks[i].gameObject.activeInHierarchy && !this.bricks[i].unbreakable)
            {
                return false;
            }
        }
        return true;
    }
}
