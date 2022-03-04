using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTextFixRotation : MonoBehaviour
{
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private BallMover _ballMover;
    private Quaternion _rotationQuaternion;

    private void Awake()
    {
        _rotationQuaternion = Quaternion.Euler(_rotation);
        FixRotation();
    }

    private void OnEnable()
    {
        _ballMover.HittedObject += OnHittedObject;
    }

    private void OnDisable()
    {
        _ballMover.HittedObject -= OnHittedObject;
    }

    private void OnHittedObject(GameObject _)
    {
        FixRotation();
    }

    private void FixRotation()
    {
        transform.rotation = _rotationQuaternion;
    }
}
