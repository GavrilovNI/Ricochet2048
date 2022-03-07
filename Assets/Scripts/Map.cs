using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map Instance { get; private set; }

    public event System.Action Lost;
    public event System.Action Won;
    
    private enum State
    {
        Playing,
        Won,
        Lost
    }
    private State _state;

    [SerializeField] private Ball _ballPrefab;

    [SerializeField] private BallContainer _balls;
    [SerializeField] private BrickContainer _bricks;
    [SerializeField] private MapSettings _mapSettings;

    public BallContainer Balls => _balls;
    public BrickContainer Bricks => _bricks;
    public MapSettings MapSettings
    {
        get => _mapSettings;
        set => _mapSettings = value;
    }

    private void Awake()
    {
        if (Instance != null)
            throw new System.Exception("Can't create second map, it should be a Singleton.");
        Instance = this;

        _state = State.Playing;
        UpdateState();
    }

    private void OnEnable()
    {
        _balls.Removed += OnBallRemoved;
        _bricks.Removed += OnBrickRemoved;
    }

    private void OnDisable()
    {
        _balls.Removed -= OnBallRemoved;
        _bricks.Removed -= OnBrickRemoved;
    }

    private void UpdateState()
    {
        if (_state != State.Playing)
            return;
        if (_bricks.Count == 0)
        {
            _state = State.Won;
            Won?.Invoke();
            return;
        }
        if (_balls.Count == 0)
        {
            _state = State.Lost;
            Lost?.Invoke();
            return;
        }
    }

    private void OnBallRemoved(Ball ball)
    {
        GameObject.Destroy(ball.gameObject);
        UpdateState();
    }
    private void OnBrickRemoved(Brick brick)
    {
        GameObject.Destroy(brick.gameObject);
        UpdateState();
    }

    public void ConvertBrickToBall(Brick brick)
    {
        Ball ball = Instantiate(_ballPrefab, brick.transform.position, Quaternion.AngleAxis(Random.value * 360f, Vector3.back)); ;
        ball.Level = brick.Level;
        ball.Radius = _mapSettings.BallRadius;
        _balls.Add(ball);

        _bricks.Remove(brick);
    }
}
