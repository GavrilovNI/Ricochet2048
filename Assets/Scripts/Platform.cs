using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float Width
    {
        get => transform.localScale.x;
        set => transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);
    }
}
