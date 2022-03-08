using UnityEngine;

[System.Serializable]
public class LevelSettings
{
    [SerializeField] private Vector2 _ballSpawnPosition = new Vector2(2, 0);
    [SerializeField] private Vector2 _brickSize = Vector2.one;
    [SerializeField] private MapSettings _mapSettings = new MapSettings();
    [SerializeField] private Vector2Int _mapSizeInBricks = Vector2Int.one * 4;
    [SerializeField, Min(0f)] private float _platformSpacingAbove = 0.5f;
    [SerializeField, Min(0f)] private float _platformSpacingUnder = 0.5f;
    [SerializeField, Min(0f)] private float _platformWidth = 1;
    [SerializeField, Min(0.01f)] private float _wallsWidth = 0.05f;

    public Vector2 BallSpawnPosition
    {
        get => _ballSpawnPosition;
        set => _ballSpawnPosition = value;
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
    public MapSettings MapSettings => _mapSettings;
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

    public Vector2 MapSize => _mapSizeInBricks * _brickSize;

    public LevelSettings()
    {
    }

    public LevelSettings(LevelSettings other)
    {
        _ballSpawnPosition = other._ballSpawnPosition;
        _brickSize = other._brickSize;
        _mapSettings = new MapSettings(other._mapSettings);
        _mapSizeInBricks = other._mapSizeInBricks;
        _platformSpacingAbove = other._platformSpacingAbove;
        _platformSpacingUnder = other._platformSpacingUnder;
        _platformWidth = other._platformWidth;
        _wallsWidth = other._wallsWidth;

    }
}

