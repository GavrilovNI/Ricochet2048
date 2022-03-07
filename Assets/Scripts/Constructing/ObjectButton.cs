using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ObjectButton : MonoBehaviour
{
    public UnityEvent MouseClick;
    public UnityEvent MouseDown;
    public UnityEvent MouseUp;
    public UnityEvent MouseEnter;
    public UnityEvent MouseExit;
    public UnityEvent MouseDrag;
    public UnityEvent MouseOver;

    [SerializeField] private float _clickDelta = 0.5f;
    private float _lastMouseDownTime;

    private void OnMouseDown()
    {
        MouseDown?.Invoke();
        _lastMouseDownTime = Time.realtimeSinceStartup;
    }

    private void OnMouseUp()
    {
        MouseUp?.Invoke();
        float currentTime = Time.realtimeSinceStartup;
        if(currentTime - _lastMouseDownTime < _clickDelta)
            MouseClick?.Invoke();
    }

    private void OnMouseEnter()
    {
        MouseEnter?.Invoke();
    }

    private void OnMouseExit()
    {
        MouseExit?.Invoke();
    }

    private void OnMouseDrag()
    {
        MouseDrag?.Invoke();
    }

    private void OnMouseOver()
    {
        MouseOver?.Invoke();
    }
}

