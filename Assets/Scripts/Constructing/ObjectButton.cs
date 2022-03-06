using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ObjectButton : MonoBehaviour
{
    public UnityEvent MouseDown;
    public UnityEvent MouseUp;
    public UnityEvent MouseEnter;
    public UnityEvent MouseExit;
    public UnityEvent MouseDrag;
    public UnityEvent MouseOver;

    private void OnMouseDown()
    {
        MouseDown?.Invoke();
    }

    private void OnMouseUp()
    {
        MouseUp?.Invoke();
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

