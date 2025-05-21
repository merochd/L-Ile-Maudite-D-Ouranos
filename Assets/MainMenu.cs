using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /*
    void Awake()
    {
        SceneManager.LoadScene(1);
        Debug.Log("Scene 1 loaded");
        SceneManager.LoadScene(1);
        Debug.Log("Scene 1 loaded");
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
        Debug.Log("Scene " + SceneManager.GetSceneAt(0) + " active");
    }
    */

    public void ResetTheGame()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        print("le jeu se relance correctement");

    }
    public void ResumeGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


}
