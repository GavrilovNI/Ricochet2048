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
                throw new Exception("Level can't be less than 0.");
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

    private void OnHittedObject(GameObject gameObject)
    {
        Brick brick = gameObject.GetComponentInParent<Brick>();
        if (brick == null)
            return;

        if(brick.Level == _level)
        {
            IncreaseLevel();
            Map.Instance.Bricks.Remove(brick);
        }
        else if(brick.Level < _level)
        {
            GameObject.Destroy(brick.gameObject);
            Map.Instance.ConvertBrickToBall(brick);
        }
    }
}
