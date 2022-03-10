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
    [SerializeField] private MapPauser _mapPauser;
    [SerializeField] private MapSettings _mapSettings;

    private List<Ball> _maxLevelBalls = new List<Ball>();
    private int _maxLevel = -1;

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
        _balls.Added += OnBallAdded;
        _bricks.Removed += OnBrickRemoved;
    }

    private void OnDisable()
    {
        _balls.Removed -= OnBallRemoved;
        _balls.Added -= OnBallAdded;
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
        _mapPauser.Continue();
        MarkMaxLevelBalls();
    }

    public void Pause()
    {
        if(_state != GameState.Playing)
            return;
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
            return;
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
            _mapPauser.Pause();
            _state = GameState.Won;
            Won?.Invoke();
            return;
        }
        if (_balls.Count == 0)
        {
            _mapPauser.Pause();
            _state = GameState.Lost;
            Lost?.Invoke();
            return;
        }
    }

    private void MarkMaxLevelBalls()
    {
        _maxLevelBalls.Clear();
        _maxLevel = -1;
        foreach(Ball ball in _balls)
        {
            AddToMaxLevelIfMax(ball);
        }
    }
    private void AddToMaxLevelIfMax(Ball ball)
    {
        Level level = ball.Level;
        if(level > _maxLevel)
        {
            foreach(Ball oldBall in _maxLevelBalls)
                oldBall.SetColor(Color.white);
            _maxLevelBalls.Clear();
            _maxLevel = level;
        }
        if(level == _maxLevel)
        {
            ball.SetColor(Color.red);
            _maxLevelBalls.Add(ball);
        }
    }

    private void OnBallLevelUpdated(Ball ball, Level level)
    {
        AddToMaxLevelIfMax(ball);
    }
    private void OnBallAdded(Ball ball)
    {
        ball.LevelUpdated += OnBallLevelUpdated;
        AddToMaxLevelIfMax(ball);
    }
    private void OnBallRemoved(Ball ball)
    {
        _maxLevelBalls.Remove(ball);
        if(_maxLevelBalls.Count == 0)
            MarkMaxLevelBalls();

        GameObject.Destroy(ball.gameObject);
        UpdatePlayingState();
    }
    private void OnBrickRemoved(Brick brick)
    {
        GameObject.Destroy(brick.gameObject);
        UpdatePlayingState();
    }

    public void SetupBall(Ball ball)
    {
        ball.Radius = _mapSettings.BallRadius;

        BallMover ballMover = ball.GetComponent<BallMover>();
        if(ballMover != null)
        {
            ballMover.Speed = _mapSettings.BallSpeed;
        }
    }

    public void ConvertBrickToBall(Brick brick)
    {
        Ball ball = Instantiate(_ballPrefab, brick.transform.position, Quaternion.AngleAxis(Random.value * 360f, Vector3.back)); ;
        ball.Level = brick.Level;
        SetupBall(ball);
        _balls.Add(ball);
        _bricks.Remove(brick);
    }
}
