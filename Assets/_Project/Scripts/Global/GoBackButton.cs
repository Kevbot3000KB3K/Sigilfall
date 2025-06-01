using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBackButton : MonoBehaviour
{
    [Tooltip("Name of the scene to return to when Back is pressed.")]
    public string sceneToLoad;

    [Tooltip("Should music be stopped when going back?")]
    public bool stopMusic = false;

    public void OnBackButtonPressed()
    {
        if (stopMusic)
        {
            StopMusicIfExists();
        }

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene name set in GoBackButton script.");
        }
    }

    private void StopMusicIfExists()
    {
        GameObject musicObj = GameObject.Find("Planet Select Audio"); 
        if (musicObj != null)
        {
            AudioSource music = musicObj.GetComponent<AudioSource>();
            if (music != null)
            {
                music.Stop();
            }
            Destroy(musicObj);
        }
    }
}
