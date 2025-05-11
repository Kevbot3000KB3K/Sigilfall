using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public float scrollSpeed = 1f;

    private Transform[] parts;
    private float spriteWidth;

    void Start()
    {
        parts = new Transform[2];
        parts[0] = transform.GetChild(0);
        parts[1] = transform.GetChild(1);
        SpriteRenderer sr = parts[0].GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;
    }

    void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;
        float camLeftEdge = Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect);
        foreach (Transform part in parts)
        {
            float partRightEdge = part.position.x + (spriteWidth / 2f);

            if (partRightEdge < camLeftEdge)
            {
                Transform other = part == parts[0] ? parts[1] : parts[0];
                part.position = new Vector3(other.position.x + spriteWidth, part.position.y, part.position.z);
            }
        }
    }
}
