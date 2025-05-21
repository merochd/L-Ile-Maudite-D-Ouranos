using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{

    public void ResetTheGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        print("le jeu se relance correctement");

    }


    public class MainMenu : MonoBehaviour
    {
        public void LoadMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
