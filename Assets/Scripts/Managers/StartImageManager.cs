using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartImageManager : MonoBehaviour
{
    public GameObject startImage; 
    private bool isLevelScene; 

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        startImage.SetActive(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        isLevelScene = scene.name.StartsWith("Stage "); 
        if (isLevelScene)
        {
            ShowStartImage();
        }
        else
        {
            startImage.SetActive(false); 
        }
    }

    public void ShowStartImage()
    {
        startImage.SetActive(true);
        Invoke(nameof(FadeOutStartImage), 2f);
    }

    private void FadeOutStartImage()
    {
        startImage.SetActive(false);
        GameManager.Instance.ball.Launch();
    }
}
