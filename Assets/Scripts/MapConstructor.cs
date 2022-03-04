using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TransformExtensions;
using VectorExtensions;

[RequireComponent(typeof(Map))]
public class MapConstructor : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [SerializeField] private Transform _leftWall;
    [SerializeField] private Transform _rightWall;
    [SerializeField] private Transform _topWall;

    [SerializeField] private BallRemover _bottomBallRemover;

    [SerializeField, Min(0f)] private float _wallsWidth = 0.05f;
    [SerializeField, Min(1f)] private float _brickSize = 1f;
    [SerializeField, Min(1f)] private int _mapWidthInBricks = 4;
    [SerializeField] private Brick _brickPrefab;

    [SerializeField] private Platform _platform;
    [SerializeField, Min(0f)] private float _platformSpacingDown = 0.5f;
    [SerializeField, Min(0f)] private float _platformSpacingUp = 1f;

    private float MapWidth => _mapWidthInBricks * _brickSize;
    private float MapHeight => MapWidth / _camera.aspect;

    private int MapHeightInBricks => Mathf.FloorToInt(MapHeight / _brickSize -
                                     (_platformSpacingDown + _platformSpacingUp));

    private Map _map;

    private void Awake()
    {
        _map = GetComponent<Map>();
    }

    private void Start()
    {
        Construct();
        SpawnTestBricks();
    }

    private void SpawnTestBricks()
    {
        int level = MapHeightInBricks * _mapWidthInBricks - 1;
        for(int y = 0; y < MapHeightInBricks; y++)
            for(int x = 0; x < _mapWidthInBricks; x++)
                SpawnBrick(new Vector2Int(x, y), _brickPrefab, (Level)level--);
    }

    private void SpawnBrick(Vector2Int position, Brick brickPrefab, Level level)
    {
        if(position.x < 0 || position.y < 0 ||
           position.x >= _mapWidthInBricks || position.y >= MapHeightInBricks)
            throw new System.ArgumentOutOfRangeException(nameof(position));

        Vector3 firstBrickPosition = Vector3.left * (MapWidth / 2f - _brickSize / 2f) +
                                     Vector3.up * (MapHeight / 2f - _brickSize / 2f);
        position.y = -position.y;
        Vector3 spawnPosition = firstBrickPosition + position.ToV3().Mply(_brickSize);

        Brick brick = Instantiate(brickPrefab, spawnPosition, Quaternion.identity);
        brick.transform.SetGlobalScale(Vector3.one * _brickSize);

        brick.Level = level;
        _map.Bricks.Add(brick);
    }

    [ContextMenu("Construct")]
    private void Construct()
    {
        float width = MapWidth;
        float height = MapHeight;

        _camera.orthographicSize = width;
        _camera.transform.rotation = Quaternion.identity;

        Vector3 cameraPosition = _camera.transform.position;

        Vector3 sideWallsScale = new Vector3(_wallsWidth, height, 1);

        _leftWall.SetGlobalScale(sideWallsScale);
        _rightWall.SetGlobalScale(sideWallsScale);
        _topWall.SetGlobalScale(new Vector3(width + _wallsWidth * 2, _wallsWidth, 1));
        _leftWall.transform.position = Vector3.left * (width / 2f + _wallsWidth / 2f);
        _rightWall.transform.position = -_leftWall.transform.position;
        _topWall.transform.position = Vector3.up * (height / 2f + _wallsWidth / 2f);

        Vector3 cameraPositionXY = new Vector3(cameraPosition.x, cameraPosition.y, 0);
        _leftWall.transform.position += cameraPositionXY;
        _rightWall.transform.position += cameraPositionXY;
        _topWall.transform.position += cameraPositionXY;

        _bottomBallRemover.transform.position = Vector3.down * (height / 2f) + cameraPositionXY;
        _bottomBallRemover.transform.SetGlobalScale(new Vector3(width + _wallsWidth * 2, 1, 1));

        Vector3 platfowmPosition = Vector3.down * (height / 2f - _platformSpacingDown) + cameraPositionXY;
        _platform.transform.position = platfowmPosition;
        PlatformMover platformMover = _platform.GetComponent<PlatformMover>();
        if(platformMover != null)
        {
            platformMover.From = platfowmPosition + Vector3.left * width / 2f + cameraPositionXY;
            platformMover.To = platfowmPosition + Vector3.right * width / 2f + cameraPositionXY;
        }
    }


}
