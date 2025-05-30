using UnityEngine;

public class Begining : MonoBehaviour
{
    public GameObject uibegining;
    private bool isCanvasVisible = false;

    void Start()
    {
        Time.timeScale = 0f;
        // Active le canvas au démarrage
        if (uibegining != null)
        {
            uibegining.SetActive(true);
            isCanvasVisible = true;
            Debug.Log("Canva bien activé");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Touche E bien détectée");
            Time.timeScale = 1f;
            if (uibegining != null)
            {
                isCanvasVisible = !isCanvasVisible;
                uibegining.SetActive(isCanvasVisible);
                Debug.Log("Destruction...");
                Destroy(this);
            }
        }
    }
}
