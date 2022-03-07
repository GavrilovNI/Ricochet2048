using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSaveScriptableObject : ScriptableObject
{
    public MapSettings Settings;
    public MapSave Save;
}

[System.Serializable]
public class MapSettings
{
    [SerializeField, Min(0.01f)] private float _wallsWidth = 0.05f;
    [SerializeField] private Vector2 _brickSize = Vector2.one;
    [SerializeField] private Vector2Int _mapSizeInBricks = Vector2Int.one * 4;
    [SerializeField, Min(0f)] private float _platformSpacingAbove = 0.5f;
    [SerializeField, Min(0f)] private float _platformSpacingUnder = 0.5f;
    [SerializeField, Min(0f)] private float _platformWidth = 1;
    [SerializeField] private Vector2 _ballSpawnPosition = new Vector2(2, 1);
    [SerializeField] private float _ballRadius = 0.15f;

    public float WallsWidth
    {
        get => _wallsWidth;
        set
        {
            if(value < 0.01f)
                throw new System.ArgumentOutOfRangeException(nameof(WallsWidth));
            else
                _wallsWidth = value;
        }
    }
    public Vector2 BrickSize
    {
        get => _brickSize;
        set
        {
            if(value.x <= 0f || value.y <= 0f)
                throw new System.ArgumentOutOfRangeException(nameof(BrickSize));
            else
                _brickSize = value;
        }
    }
    public Vector2Int MapSizeInBricks
    {
        get => _mapSizeInBricks;
        set
        {
            if(value.x <= 0f || value.y <= 0f)
                throw new System.ArgumentOutOfRangeException(nameof(MapSizeInBricks));
            else
                _mapSizeInBricks = value;
        }
    }
    public float PlatformSpacingAbove
    {
        get => _platformSpacingAbove;
        set
        {
            if(value < 0f)
                throw new System.ArgumentOutOfRangeException(nameof(PlatformSpacingAbove));
            else
                _platformSpacingAbove = value;
        }
    }
    public float PlatformSpacingUnder
    {
        get => _platformSpacingUnder;
        set
        {
            if(value < 0f)
                throw new System.ArgumentOutOfRangeException(nameof(PlatformSpacingUnder));
            else
                _platformSpacingUnder = value;
        }
    }
    public float PlatformWidth
    {
        get => _platformWidth;
        set
        {
            if(value < 0f)
                throw new System.ArgumentOutOfRangeException(nameof(PlatformWidth));
            else
                _platformWidth = value;
        }
    }
    public Vector2 BallSpawnPosition
    {
        get => _ballSpawnPosition;
        set
        {
            _ballSpawnPosition = value;
        }
    }
    public float BallRadius
    {
        get => _ballRadius;
        set
        {
            if(value < 0f)
                throw new System.ArgumentOutOfRangeException(nameof(BallRadius));
            else
                _ballRadius = value;
        }
    }

    public Vector2 MapSize => _mapSizeInBricks * _brickSize;
}


[System.Serializable]
public class MapSave
{
    private Dictionary<Vector2Int, Level> _bricks = new Dictionary<Vector2Int, Level>();

    public void Set(Vector2Int position, Level? level)
    {
        if(_bricks.ContainsKey(position))
        {
            if(level == null)
                _bricks.Remove(position);
            else
                _bricks[position] = level.Value;
        }
        else
        {
            if(level != null)
                _bricks.Add(position, level.Value);
        }
    }
}
