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

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (_rigidbody.freezeRotation == false)
            Debug.LogWarning("Ball rotation must be frozen.");
        if (_rigidbody.simulated == false)
            Debug.LogWarning("Ball rigidbody must be simulated.");
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = transform.up * _speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 commonNormal = Vector3.zero;
        foreach (var contact in collision.contacts)
            commonNormal += contact.normal;
        commonNormal.Normalize();
        transform.up = Vector3.Reflect(transform.up, commonNormal);

        HittedObject?.Invoke(collision.gameObject);
    }
}
