using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public event System.Action<Level> LevelUpdated;

    [SerializeField] private Level _level;

    public Level Level
    {
        get => _level;
        set
        {
            _level = value;
            LevelUpdated(_level);
        }
    }
}
