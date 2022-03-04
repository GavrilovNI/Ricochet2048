using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Platform))]
public class PlatformMover : MonoBehaviour
{
    private Platform _platform;

    [SerializeField] private Vector3 _from;
    [SerializeField] private Vector3 _to;
    [SerializeField, Min(0f)] private float _speed = 3f;

    private Vector3 MinPosition => _from + Vector3.right * _platform.Width / 2;
    private Vector3 MaxPosition => _to - Vector3.right * _platform.Width / 2;

    private const string InputAxisName = "Horizontal";
    private float _input = 0;

    private void Awake()
    {
        _platform = GetComponent<Platform>();
    }

    private void Update()
    {
        _input = Input.GetAxisRaw(InputAxisName);
    }

    private void FixedUpdate()
    {
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

        Vector3 delta = newPosition - transform.position;
        transform.Translate(delta, Space.World);
    }
}
