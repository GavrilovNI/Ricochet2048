using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotation : MonoBehaviour
{
    [SerializeField] private Vector3 _rotation;
    private Quaternion _rotationQuaternion;

    private void Start()
    {
        _rotationQuaternion = Quaternion.Euler(_rotation);
    }

    private void FixedUpdate()
    {
        transform.rotation = _rotationQuaternion;
    }
}
