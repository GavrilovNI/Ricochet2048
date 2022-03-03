using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BallMover))]
public class Ball : MonoBehaviour
{
    public event Action<int> LevelUpdated;

    private BallMover _ballMover;

    [SerializeField, Min(0)] private int _level;

    public int Level
    {
        get => _level;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("Level can't be less than 0.");
            _level = value;
            LevelUpdated(_level);
        }
    }

    

    private void Awake()
    {
        _ballMover = GetComponent<BallMover>();
    }

    private void OnEnable()
    {
        _ballMover.HittedObject += OnHittedObject;
    }

    private void OnDisable()
    {
        _ballMover.HittedObject -= OnHittedObject;
    }

    private void IncreaseLevel()
    {
        Level++;
    }

    private void OnHittedObject(Collider2D collider)
    {
        Brick brick = collider.GetComponentInParent<Brick>();
        if (brick == null)
            return;

        if(brick.Level == _level)
        {
            IncreaseLevel();
            Map.Instance.RemoveBrick(brick);
        }
        else if(brick.Level < _level)
        {
            GameObject.Destroy(brick.gameObject);
            Map.Instance.ConvertBrickToBall(brick);
        }
    }
}
