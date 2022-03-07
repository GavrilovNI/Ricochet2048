using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TransformExtensions;
using VectorExtensions;
using UnityEditor;

[RequireComponent(typeof(Map))]
public class MapField : MonoBehaviour
{
    [SerializeField] private Transform _leftWall;
    [SerializeField] private Transform _rightWall;
    [SerializeField] private Transform _topWall;
    [SerializeField] private Platform _platform;
    [SerializeField] private BallRemover _bottomBallRemover;
    [SerializeField] private Brick _brickPrefab;
    [SerializeField] private Ball _ballPrefab;

    [SerializeField] private LevelSettings _settings;

    public LevelSettings Settings => _settings;
    
    public Vector2 MapSize => _settings.MapSize;
    public Bounds MapBounds => new Bounds(transform.position + MapSize.ToV3() / 2f, MapSize);

    private Map _map;

    private void Awake()
    {
        _map = GetComponent<Map>();
    }

    private void Start()
    {
        ConstructDefault();
        //SpawnTestBricks();
    }

    [ContextMenu("Spawn Ball")]
    public void SpawnBall()
    {
        Ball ball = Instantiate(_ballPrefab);
        ball.transform.position = MapBounds.min + Settings.BallSpawnPosition.ToV3();
        ball.transform.rotation = Quaternion.identity;
        ball.Radius = Settings.MapSettings.BallRadius;
        _map.Balls.Add(ball);
    }

    private void SpawnTestBricks()
    {
        int level = 0;
        for(int y = 0; y < _settings.MapSizeInBricks.y; y++)
            for(int x = 0; x < _settings.MapSizeInBricks.x; x++)
                SpawnBrick(new Vector2Int(x, y), _brickPrefab, (Level)level++);
    }

    public Vector3 GetBrickPosition(Vector2Int position)
    {
        return MapBounds.min + (_settings.BrickSize * (position + Vector2.one * 0.5f)).ToV3();
    }

    public void SpawnBrick(Vector2Int position, Brick brickPrefab, Level level)
    {
        Vector3 spawnPosition = GetBrickPosition(position);

        if(MapBounds.Contains(spawnPosition) == false)
            throw new System.ArgumentOutOfRangeException(nameof(position));

        Brick brick = Instantiate(brickPrefab, spawnPosition, Quaternion.identity);
        brick.transform.SetGlobalScale(Vector3.one * _settings.BrickSize);

        brick.Level = level;
        _map.Bricks.Add(brick);
    }

    [ContextMenu("Construct")]
    public void ConstructDefault()
    {
        Bounds mapBounds = MapBounds;

        float fullPlatformSpacing = _settings.PlatformSpacingAbove + _settings.PlatformSpacingUnder;
        Vector3 sideWallsScale = new Vector3(_settings.WallsWidth, mapBounds.size.y + fullPlatformSpacing);
        _leftWall.SetGlobalScale(sideWallsScale);
        _rightWall.SetGlobalScale(sideWallsScale);
        _topWall.SetGlobalScale(new Vector3(mapBounds.size.x + _settings.WallsWidth * 2, _settings.WallsWidth));

        _leftWall.transform.position = new Vector3(mapBounds.min.x - _settings.WallsWidth / 2f,
                                                   mapBounds.center.y - fullPlatformSpacing / 2f);
        _rightWall.transform.position = new Vector3(mapBounds.max.x + _settings.WallsWidth / 2f,
                                                   mapBounds.center.y - fullPlatformSpacing / 2f);
        _topWall.transform.position = new Vector3(mapBounds.center.x,
                                                  mapBounds.max.y + _settings.WallsWidth / 2f);

        _bottomBallRemover.transform.position = new Vector3(mapBounds.center.x,
                                                            mapBounds.min.y - fullPlatformSpacing);
        _bottomBallRemover.transform.SetGlobalScale(new Vector3(mapBounds.size.x, 1));

        Vector3 platformPosition = new Vector3(mapBounds.center.x,
                                               mapBounds.min.y - _settings.PlatformSpacingAbove);
        _platform.transform.position = platformPosition;
        _platform.Width = Settings.PlatformWidth;
        PlatformMover platformMover = _platform.GetComponent<PlatformMover>();
        if(platformMover != null)
        {
            platformMover.From = new Vector3(mapBounds.min.x, platformPosition.y);
            platformMover.To = new Vector3(mapBounds.max.x, platformPosition.y);
        }
    }


}
