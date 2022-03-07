using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BallRemover : MonoBehaviour
{
    private void Awake()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider.isTrigger == false)
            Debug.LogWarning("BallRemover collider must be a trigger.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ball ball = collision.GetComponentInParent<Ball>();
        if (ball == null)
            return;

        Map.Instance.Balls.Remove(ball);
    }
}
