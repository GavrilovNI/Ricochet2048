using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Button2D))]
public class CreateBrickButton : BrickButton
{
    public void Awake()
    {
        GetComponent<Button2D>().MouseClick.AddListener((mouseButton) =>
        {
            if(mouseButton == MouseButton.Left)
                Switch?.Invoke();
        });
    }
}
