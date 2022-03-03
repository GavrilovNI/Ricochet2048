using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField, Min(0)] private int _level;

    public int Level => _level;
}
