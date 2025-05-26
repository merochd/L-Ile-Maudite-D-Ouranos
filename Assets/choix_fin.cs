using UnityEngine;
using UnityEngine.UI;

public class ChoiceTrigger : MonoBehaviour
{
    public GameObject mainChoiceCanvas;
    public GameObject canvasOption1;
    public GameObject canvasOption2;
    private bool hasTriggered = false;

    private void Start()
    {
        mainChoiceCanvas.SetActive(false);
        canvasOption1.SetActive(false);
        canvasOption2.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            //Time.timeScale = 0f;
            mainChoiceCanvas.SetActive(true);
            Debug.Log("le joueur est entré dans la zone");
        }
    }

    public void OnChoice1()
    {
        mainChoiceCanvas.SetActive(false);
        canvasOption1.SetActive(true);
    }

    public void OnChoice2()
    {
        mainChoiceCanvas.SetActive(false);
        canvasOption2.SetActive(true);
    }
}
