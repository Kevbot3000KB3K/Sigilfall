using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Ball Effects/Lifegain")]
public class LifegainEffect : BallEffect
{
    public float lifegainChance = 0.05f;
    public Sprite Icon;

    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (Random.value < lifegainChance)
        {
            GameManager gm = GameObject.FindObjectOfType<GameManager>();
            GameManager.Instance.GainLife();

        }
    }
}