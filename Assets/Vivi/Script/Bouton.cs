using UnityEngine;

public class Bouton : MonoBehaviour
{
    // mettre object 1 et 2 dans les parts, quand le player interagie avec le bouton (sur lequel le script sera attacher)
    // les object non actifs deviennent actif et les actif se désactivent
    // se script va sur le bouton et les objets seront dans le bouton (dans la hierarchie)
    // se script permet d'utiliser le bouton une seule fois, mais on peux faire en sorte que se soit à l'infini
    public GameObject[] parts;
    private bool aEteUtiliser = false;


    private void OnTriggerEnter(Collider other)
    {
        if (!aEteUtiliser && other.CompareTag("Player"))
        {
            SwitchState();
            aEteUtiliser = true;
        }
    }

    public void SwitchState()
    {

        foreach (GameObject obj in parts)
            if (obj.activeInHierarchy)
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }
    }
}
