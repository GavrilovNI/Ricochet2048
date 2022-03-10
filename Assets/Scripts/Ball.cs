using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TransformExtensions;

[RequireComponent(typeof(BallMover))]
public class Ball : MonoBehaviour
{
    public event Action<Ball, Level> LevelUpdated;

    private BallMover _ballMover;

    [SerializeField] private Level _level;
    [SerializeField, Min(0f)] private float _radius = 0.15f;
    [SerializeField] private SpriteRenderer _model;

    public Level Level
    {
        get => _level;
        set
        {
            _level = value;
            LevelUpdated(this, _level);
        }
    }
    public float Radius
    {
        get => _radius;
        set
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException(nameof(Radius));
            else
            {
                _radius = value;
                UpdateRadius();
            }
        }
    }


    private void Awake()
    {
        _ballMover = GetComponent<BallMover>();
    }
    private void Start()
    {
        UpdateRadius();
    }

    private void OnEnable()
    {
        _ballMover.HittedObject += OnHittedObject;
    }

    private void OnDisable()
    {
        _ballMover.HittedObject -= OnHittedObject;
    }

    public void SetColor(Color color)
    {
        _model.color = color;
    }

    private void UpdateRadius()
    {
        transform.SetGlobalScale(Vector3.one * _radius * 2f);
    }

    private void IncreaseLevel()
    {
        Level = (Level)(Level + 1);
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
