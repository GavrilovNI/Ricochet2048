using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMover : MonoBehaviour
{
    [SerializeField] private string _moveButton = "Fire3";
    [SerializeField] private string _zoomButton = "Mouse ScrollWheel";

    [SerializeField, Min(0f)] private float _moveSensitivity = 1;
    [SerializeField, Min(0f)] private float _scrollSensitivity = 1;
    [SerializeField, Min(0f)] private float _minHoldTime = 0.05f; // time needed to hold before moving
    [SerializeField, Min(0f)] private float _minMoveDelta = 2; // pixels needed to move from click before moving

    private Camera _camera;
    private Vector3 _oldMousePosition;

    private bool _isFocused;

    private Vector3 _mouseDownPosition;
    private bool _buttonDown = false;
    private bool _movedMoreThanDelta = false;
    private float _timeOnButtonDown;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _oldMousePosition = Input.mousePosition;
    }

    private void OnApplicationFocus(bool focus)
    {
        _isFocused = focus;
    }

    private void Update()
    {
        if(_isFocused == false)
            return;

        if(Input.GetButtonDown(_moveButton))
        {
            _buttonDown = true;
            _timeOnButtonDown = Time.realtimeSinceStartup;
            _mouseDownPosition = Input.mousePosition;
            _movedMoreThanDelta = false;
        }
        if(Input.GetButtonUp(_moveButton))
        {
            _buttonDown = false;
        }

        float timeSinceButtonDown = Time.realtimeSinceStartup - _timeOnButtonDown;

        if(_movedMoreThanDelta == false)
            _movedMoreThanDelta = Vector3.Distance(Input.mousePosition, _mouseDownPosition) >= _minMoveDelta;

        if(_buttonDown && timeSinceButtonDown > _minHoldTime && _movedMoreThanDelta && Input.GetButton(_moveButton))
        {
            Vector2 delta = _camera.ScreenToWorldPoint(Input.mousePosition) - _camera.ScreenToWorldPoint(_oldMousePosition);
            transform.Translate(-delta * _moveSensitivity);

            _oldMousePosition = Input.mousePosition;
        }
        float zoomAxisDelta = Input.GetAxis(_zoomButton);
        float newOrthographicSize = _camera.orthographicSize - zoomAxisDelta * _scrollSensitivity;
        if(newOrthographicSize > 0)
            _camera.orthographicSize = newOrthographicSize;

        if(Input.GetButtonDown(_moveButton))
        {
            _oldMousePosition = Input.mousePosition;
        }
    }
}
