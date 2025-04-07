using UnityEngine;

public class PlateformMove : MonoBehaviour
{
    public float floatAmplitude = 0.5f;
    public float floatSpeed = 1f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
