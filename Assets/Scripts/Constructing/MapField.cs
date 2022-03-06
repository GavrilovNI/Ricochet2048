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

    [SerializeField, Min(0f)] private float _wallsWidth = 0.05f;
    [SerializeField, Min(1f)] private Vector2 _brickSize = Vector2.one;
    [SerializeField] private Vector2Int _mapSizeInBricks = Vector2Int.one * 4;
    [SerializeField, Min(0f)] private float _platformSpacingAbove = 0.5f;
    [SerializeField, Min(0f)] private float _platformSpacingUnder = 0.5f;

    public Vector2 BrickSize
    {
        get { return _brickSize; }
        set
        {
            if(value.x < 0 || value.y < 0)
                throw new System.Exception("Brick size must be moew than (0, 0).");
            _brickSize = value;
        }
    }
    public Vector2Int MapSizeInBricks
    {
        get { return _mapSizeInBricks; }
        set {
            if(value.x < 0 || value.y < 0)
                throw new System.Exception("Map size must be moew than (0, 0).");
            _mapSizeInBricks = value;
        }
    }
    
    public Vector2 MapSize => _mapSizeInBricks * _brickSize;
    public Bounds MapBounds => new Bounds(transform.position + MapSize.ToV3() / 2f, MapSize);

    private Map _map;

    private void Awake()
    {
        _map = GetComponent<Map>();
    }

    private void Start()
    {
        Construct();
        //SpawnTestBricks();
    }

    private void SpawnTestBricks()
    {
        int level = 0;
        for(int y = 0; y < _mapSizeInBricks.y; y++)
            for(int x = 0; x < _mapSizeInBricks.x; x++)
                SpawnBrick(new Vector2Int(x, y), _brickPrefab, (Level)level++);
    }

    public Vector3 GetBrickPosition(Vector2Int position)
    {
        return MapBounds.min + (_brickSize * (position + Vector2.one * 0.5f)).ToV3();
    }

    public void SpawnBrick(Vector2Int position, Brick brickPrefab, Level level)
    {
        Vector3 spawnPosition = GetBrickPosition(position);

        if(MapBounds.Contains(spawnPosition) == false)
            throw new System.ArgumentOutOfRangeException(nameof(position));

        Brick brick = Instantiate(brickPrefab, spawnPosition, Quaternion.identity);
        brick.transform.SetGlobalScale(Vector3.one * _brickSize);

        brick.Level = level;
        _map.Bricks.Add(brick);
    }

    [ContextMenu("Construct")]
    public void Construct()
    {
        Bounds mapBounds = MapBounds;

        float fullPlatformSpacing = _platformSpacingAbove + _platformSpacingUnder;
        Vector3 sideWallsScale = new Vector3(_wallsWidth, mapBounds.size.y + fullPlatformSpacing);
        _leftWall.SetGlobalScale(sideWallsScale);
        _rightWall.SetGlobalScale(sideWallsScale);
        _topWall.SetGlobalScale(new Vector3(mapBounds.size.x + _wallsWidth * 2, _wallsWidth));

        _leftWall.transform.position = new Vector3(mapBounds.min.x - _wallsWidth / 2f,
                                                   mapBounds.center.y - fullPlatformSpacing / 2f);
        _rightWall.transform.position = new Vector3(mapBounds.max.x + _wallsWidth / 2f,
                                                   mapBounds.center.y - fullPlatformSpacing / 2f);
        _topWall.transform.position = new Vector3(mapBounds.center.x,
                                                  mapBounds.max.y + _wallsWidth / 2f);

        _bottomBallRemover.transform.position = new Vector3(mapBounds.center.x,
                                                            mapBounds.min.y - fullPlatformSpacing);
        _bottomBallRemover.transform.SetGlobalScale(new Vector3(mapBounds.size.x, 1));

        Vector3 platformPosition = new Vector3(mapBounds.center.x,
                                               mapBounds.min.y - _platformSpacingAbove);
        _platform.transform.position = platformPosition;
        PlatformMover platformMover = _platform.GetComponent<PlatformMover>();
        if(platformMover != null)
        {
            platformMover.From = new Vector3(mapBounds.min.x, platformPosition.y);
            platformMover.To = new Vector3(mapBounds.max.x, platformPosition.y);
        }
    }


}
