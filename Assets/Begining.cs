using UnityEngine;

public class Begining : MonoBehaviour
{
    public GameObject uibegining;
    private bool isCanvasVisible = false;

    void Start()
    {
        // Active le canvas au démarrage
        if (uibegining != null)
        {
            uibegining.SetActive(true);
            isCanvasVisible = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (uibegining != null)
            {
                isCanvasVisible = !isCanvasVisible;
                uibegining.SetActive(isCanvasVisible);
            }
        }
    }
}
