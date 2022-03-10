using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Platform))]
public class PlatformMover : MonoBehaviour, IPausable
{
    private Platform _platform;
    private Rigidbody2D _rigidbody;

    [SerializeField] private Vector3 _from;
    [SerializeField] private Vector3 _to;
    [SerializeField, Min(0f)] private float _speed = 2.5f;

    private Vector3 MinPosition => _from + Vector3.right * _platform.Width / 2;
    private Vector3 MaxPosition => _to - Vector3.right * _platform.Width / 2;

    public const string InputAxisName = "Horizontal";
    private float _input = 0;

    private bool _paused = false;

    public float Speed
    {
        get => _speed;
        set
        {
            if(value < 0f)
                throw new System.ArgumentOutOfRangeException(nameof(Speed));
            else
                _speed = value;
        }
    }
    public Vector3 From
    {
        get => _from;
        set => _from = value;
    }
    public Vector3 To
    {
        get => _to;
        set => _to = value;
    }

    public bool IsPaused => _paused;

    private void Awake()
    {
        _platform = GetComponent<Platform>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(_paused)
            return;
        _input = Input.GetAxisRaw(InputAxisName);
    }

    private void FixedUpdate()
    {
        if(_paused)
            return;
        static bool IsLess(Vector3 a, Vector3 b) =>
            a.x < b.x || a.y < b.y || a.z < b.z;
        static bool IsBigger(Vector3 a, Vector3 b) =>
            a.x > b.x || a.y > b.y || a.z > b.z;
        static Vector3 Clamp(Vector3 vector3, Vector3 min, Vector3 max)
        {
            if(IsLess(vector3, min))
                return min;
            else if(IsBigger(vector3, max))
                return max;
            return vector3;
        }

        Vector3 newPosition;

        if(IsLess(MaxPosition, MinPosition))
        {
            newPosition = (MinPosition + MaxPosition) / 2f;
        }
        else
        {
            Vector3 velocity = _input * Vector3.right;
            newPosition = transform.position + velocity * _speed * Time.fixedDeltaTime;
            newPosition = Clamp(newPosition, MinPosition, MaxPosition);
        }

        _rigidbody.MovePosition(newPosition.XY());
    }

    public void Pause() => _paused = true;
    public void Continue() => _paused = false;
}
