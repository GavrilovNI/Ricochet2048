using UnityEngine;

[System.Serializable]
public class MapSettings
{
    [SerializeField] private float _ballRadius = 0.15f;
    [SerializeField] private float _platformWidth = 1f;

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

    public MapSettings()
    {

    }

    public MapSettings(MapSettings other)
    {
        _ballRadius = other._ballRadius;
        _platformWidth = other._platformWidth;
    }
}

