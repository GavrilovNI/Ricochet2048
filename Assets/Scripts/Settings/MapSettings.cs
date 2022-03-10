using UnityEngine;

[System.Serializable]
public class MapSettings
{
    [SerializeField, Min(0f)] private float _ballRadius = 0.15f;
    [SerializeField, Min(0f)] private float _ballSpeed = 3f;

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
    public float BallSpeed
    {
        get => _ballSpeed;
        set
        {
            if(value < 0f)
                throw new System.ArgumentOutOfRangeException(nameof(BallSpeed));
            else
                _ballSpeed = value;
        }
    }

    public MapSettings()
    {

    }

    public MapSettings(MapSettings other)
    {
        _ballRadius = other._ballRadius;
        _ballSpeed = other._ballSpeed;
    }
}

