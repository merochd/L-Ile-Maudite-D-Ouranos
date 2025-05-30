using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChoiceTrigger : MonoBehaviour
{
    public GameObject mainChoiceCanvas;
    public GameObject ChoiceCanvas1;
    public GameObject ChoiceCanvas2;
    public GameObject canvasOption1;
    public GameObject canvasOption2;
    private bool hasTriggered = false;

    private void Start()
    {
        mainChoiceCanvas.SetActive(false);
        ChoiceCanvas1.SetActive(false);
        ChoiceCanvas2.SetActive(false);
        canvasOption1.SetActive(false);
        canvasOption2.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            Time.timeScale = 0f;
            mainChoiceCanvas.SetActive(true);
            ChoiceCanvas1.SetActive(true);
            ChoiceCanvas2.SetActive(true);
            Debug.Log("le joueur est entré dans la zone");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void OnChoice1()
    {
        //mainChoiceCanvas.SetActive(false);
        ChoiceCanvas1.SetActive(false);
        ChoiceCanvas2.SetActive(false);
        canvasOption1.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Time.timeScale = 1f;
    }

    public void OnChoice2()
    {
        //mainChoiceCanvas.SetActive(false);
        ChoiceCanvas1.SetActive(false);
        ChoiceCanvas2.SetActive(false);
        canvasOption2.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Time.timeScale = 1f;
    }

    void Update()
    {
        if (!hasTriggered) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
