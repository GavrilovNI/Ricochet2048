using System;
using UnityEngine;

[Serializable]
public class Level
{
    [SerializeField, Min(0)] private int _level;

    public Level(int level = 0)
    {
        if(level < 0)
            throw new ArgumentOutOfRangeException(nameof(level));
        _level = level;
    }

    public static implicit operator int(Level level)
    {
        return level._level;
    }

    public static explicit operator Level(int level)
    {
        return new Level(level);
    }

    public static bool operator ==(Level a, Level b) =>
        a._level == b._level;
    public static bool operator !=(Level a, Level b) =>
        a._level != b._level;

    public override bool Equals(object obj)
    {
        if(obj is null)
            return false;
        Level level = obj as Level;
        if(level is null)
            return false;
        return this == level;
    }

    public override int GetHashCode() => _level.GetHashCode();
}
