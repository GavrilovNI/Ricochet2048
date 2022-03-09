using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour, IPausable
{
    public static Map Instance { get; private set; }

    public event System.Action Lost;
    public event System.Action Won;
    
    public enum GameState
    {
        Cleared,
        Paused,
        Playing,
        Won,
        Lost,
    }
    private GameState _state;

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
    public GameState State => _state;

    public bool IsPaused => _state == GameState.Paused;

    private void Awake()
    {
        if (Instance != null)
            throw new System.Exception("Can't create second map, it should be a Singleton.");
        Instance = this;

        _state = _balls.Count == 0 && _bricks.Count == 0 ? GameState.Cleared : GameState.Playing;
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

    public void Clear()
    {
        _bricks.Clear();
        _balls.Clear();
        _state = GameState.Cleared;
    }
    public void StartGame()
    {
        _state = GameState.Playing;
    }

    public void Pause()
    {
        if(_state != GameState.Playing)
            throw new System.InvalidOperationException("Can't pause. Game is not running.");
        foreach(Ball ball in _balls)
        {
            BallMover ballMover = ball.GetComponent<BallMover>();
            if(ballMover != null)
                ballMover.Pause();
        }
        _state = GameState.Paused;
    }

    public void Continue()
    {
        if(_state != GameState.Paused)
            throw new System.InvalidOperationException("Can't continue. Game is not paused.");
        foreach(Ball ball in _balls)
        {
            BallMover ballMover = ball.GetComponent<BallMover>();
            if(ballMover != null)
                ballMover.Continue();
        }
        _state = GameState.Playing;
        UpdatePlayingState();
    }

    private void UpdatePlayingState()
    {
        if (_state != GameState.Playing)
            return;
        if (_bricks.Count == 0)
        {
            _state = GameState.Won;
            Won?.Invoke();
            return;
        }
        if (_balls.Count == 0)
        {
            _state = GameState.Lost;
            Lost?.Invoke();
            return;
        }
    }

    private void OnBallRemoved(Ball ball)
    {
        GameObject.Destroy(ball.gameObject);
        UpdatePlayingState();
    }
    private void OnBrickRemoved(Brick brick)
    {
        GameObject.Destroy(brick.gameObject);
        UpdatePlayingState();
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
