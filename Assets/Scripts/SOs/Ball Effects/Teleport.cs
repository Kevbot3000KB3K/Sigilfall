using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Ball Effects/Teleport")]
public class TeleportEffect : BallEffect
{
    private float timer = 0f;
    public float interval = 10f;
    public Sprite Icon;

    public override void OnUpdate(Ball ball)
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            float x = Random.Range(-8f, 8f); // Replace with your stage width
            float y = Random.Range(0f, 5f); // Always above middle
            ball.transform.position = new Vector3(x, y, 0);
            timer = 0f;
        }
    }
}
