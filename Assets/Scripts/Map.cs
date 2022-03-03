using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map Instance { get; private set; }

    [SerializeField] private Ball _ballPrefab;

    [SerializeField] private List<Ball> _balls = new List<Ball>();
    [SerializeField] private List<Brick> _bricks = new List<Brick>();

    public void Awake()
    {
        if (Instance != null)
            throw new System.Exception("Can't create second map, it should be a Singleton.");
        Instance = this;
    }

    public void RemoveBrick(Brick brick)
    {
        _bricks.Remove(brick);
        GameObject.Destroy(brick.gameObject);
    }

    public void ConvertBrickToBall(Brick brick)
    {
        Ball ball = Instantiate(_ballPrefab, brick.transform.position, Quaternion.AngleAxis(Random.value * 360f, Vector3.back)); ;
        ball.Level = brick.Level;
        _balls.Add(ball);

        RemoveBrick(brick);
    }

    public void RemoveBall(Ball ball)
    {
        _balls.Remove(ball);
        GameObject.Destroy(ball.gameObject);
    }
}
