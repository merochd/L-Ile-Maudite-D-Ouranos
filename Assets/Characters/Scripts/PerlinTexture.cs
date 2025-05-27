using UnityEngine;

public class PerlinTexture : MonoBehaviour
{
    [SerializeField] int textureWidth = 1024;
    [SerializeField] int textureHeight = 1024;
    [SerializeField] float scale = 10f;
    Texture2D texture;

    void Start()
    {
        texture = new Texture2D(textureWidth, textureHeight);

        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        Material material = new Material(renderer.material.shader);
        material.mainTexture = texture;

        renderer.material = material;
        
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float xCoord = (float)x / textureWidth * scale;
                float yCoord = (float)y / textureHeight * scale;

                float r = Mathf.PerlinNoise(xCoord * 1f, yCoord * 1f);
                float g = Mathf.PerlinNoise(xCoord * 5f, yCoord * 5f);
                float b = Mathf.PerlinNoise(xCoord * 9f, yCoord * 9f);
                Color color = new Color(r, g, b); // niveau de gris
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
    }
}
