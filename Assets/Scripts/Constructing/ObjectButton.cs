using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public enum MouseButton
{
    Left = 0,
    Right = 1,
    Middle = 2,
}

public abstract class ObjectButton : MonoBehaviour
{

    public UnityEvent<MouseButton> MouseClick;
    public UnityEvent<MouseButton> MouseDown;
    public UnityEvent<MouseButton> MouseUp;
    public UnityEvent MouseEnter;
    public UnityEvent MouseExit;
    public UnityEvent MouseDrag;
    public UnityEvent MouseOver;

    [SerializeField] private float _clickDelta = 0.5f;
    private float _lastMouseDownTime;

    private HashSet<MouseButton> _downButtons = new HashSet<MouseButton> ();

    private void OnMouseDown()
    {
        MouseDown?.Invoke(MouseButton.Left);
        _lastMouseDownTime = Time.realtimeSinceStartup;
    }

    private void OnMouseUp()
    {
        MouseUp?.Invoke(MouseButton.Left);
        float currentTime = Time.realtimeSinceStartup;
        if(currentTime - _lastMouseDownTime < _clickDelta)
            MouseClick?.Invoke(MouseButton.Left);
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

        MouseButton[] checkButtons = new MouseButton[] { MouseButton.Right, MouseButton.Middle };

        foreach(MouseButton button in checkButtons)
        {
            if(Input.GetMouseButtonDown((int)button))
            {
                _downButtons.Add(button);
                MouseDown?.Invoke(button);
            }
        }
    }

    private void Update()
    {
        foreach(MouseButton button in _downButtons.ToList())
        {
            if(Input.GetMouseButtonUp((int)button))
            {
                _downButtons.Remove(button);
                MouseUp?.Invoke(button);
            }
        }
    }
}

