
using UnityEngine;


public class Lore : MonoBehaviour
{
    public GameObject TextLore;
    private bool isLore;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isLore = true;
            Debug.Log("joueur entré dans la zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isLore = false;
            TextLore.SetActive(false);
            Debug.Log("le joueur est sorti de la zone");
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (isLore && Input.GetKeyDown(KeyCode.E))
        {
            TextLore.SetActive(!TextLore.activeSelf);
            if (TextLore.activeSelf)
            {
                Debug.Log("Texte affiché");
            }
            else
                Debug.Log("Texte masqué");
        }
    }
}
