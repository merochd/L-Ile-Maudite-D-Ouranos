using UnityEngine;
using UnityEngine.Events;

public class PressE : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnEPressed;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnEPressed.Invoke();
        }
    }
}
