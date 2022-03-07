using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeSizeButtons : MonoBehaviour
{
    public enum ChangeDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public static ChangeDirection[] Directions => (ChangeSizeButtons.ChangeDirection[])System.Enum.GetValues(typeof(ChangeSizeButtons.ChangeDirection));

    [SerializeField] private ChangeDirection _direction;
    [SerializeField] private Button2D _increaseButton;
    [SerializeField] private Button2D _decreaseButton;

    public ChangeDirection Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            UpdateRotation();
        }
    }    
    public UnityEvent<ChangeDirection> Increase;
    public UnityEvent<ChangeDirection> Decrease;

    private void Awake()
    {
        _increaseButton.MouseDown.AddListener(() => Increase.Invoke(_direction));
        _decreaseButton.MouseDown.AddListener(() => Decrease.Invoke(_direction));

        UpdateRotation();
    }

    private void UpdateRotation()
    {
        Vector3 euler;
        switch(Direction)
        {
            case ChangeDirection.Bottom:
                euler = new Vector3(0, 0, 270);
                break;
            case ChangeDirection.Left:
                euler = new Vector3(0, 0, 180);
                break;
            case ChangeDirection.Top:
                euler = new Vector3(0, 0, 90);
                break;
            default:
            case ChangeDirection.Right:
                euler = new Vector3(0, 0, 0);
                break;
        }
        transform.rotation = Quaternion.Euler(euler);
    }
}
