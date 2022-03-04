using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallMover : MonoBehaviour
{
    public event Action<GameObject> HittedObject;

    [SerializeField, Min(0f)] private float _speed = 3;

    private Vector3 Direction
    {
        get => transform.up;
        set
        {
            float angle = Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg;
            float eulerZ = angle - 90;
            transform.rotation = Quaternion.Euler(0, 0, eulerZ);
        }
    }

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (_rigidbody.freezeRotation == false)
            Debug.LogWarning("Ball rotation must be frozen.");
        if (_rigidbody.simulated == false)
            Debug.LogWarning("Ball rigidbody must be simulated.");
        if(_rigidbody.gravityScale != 0f)
            Debug.LogWarning("Ball rigidbody gravityScale must be 0.");
    }

    private void FixedUpdate()
    {
        //_rigidbody.AddForce(Direction * _speed, ForceMode2D.Impulse);
        _rigidbody.velocity = Direction * _speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 commonNormal = Vector3.zero;
        foreach (var contact in collision.contacts)
            commonNormal += contact.normal;
        commonNormal.Normalize();

        Direction = Vector3.Reflect(transform.up, commonNormal);

        HittedObject?.Invoke(collision.gameObject);
    }
}
