using UnityEngine;

[System.Serializable]
public class BrickType
{
    [SerializeField, Min(0f)] private int _brickModelIndex;
    [SerializeField] private Level _level;

    public int BrickModelIndex => _brickModelIndex;
    public Level Level => _level;

    public BrickType(int brickModelIndex, Level level)
    {
        if(brickModelIndex < 0)
            throw new System.ArgumentOutOfRangeException(nameof(brickModelIndex));
        _brickModelIndex = brickModelIndex;
        _level = level;
    }
}

