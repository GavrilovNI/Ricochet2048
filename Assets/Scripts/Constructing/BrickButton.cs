using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TransformExtensions;

public abstract class BrickButton : MonoBehaviour
{
    public UnityEvent Switch;
    public virtual Vector2 Scale => transform.lossyScale;
    public virtual void SetScale(Vector2 scale)
    {
        transform.SetGlobalScale(scale);
    }
}
