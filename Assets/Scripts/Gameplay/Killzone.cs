using UnityEngine;

public class Killzone : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ball")
        {
            FindFirstObjectByType<GameManager>().Miss();
        }
    }
}
