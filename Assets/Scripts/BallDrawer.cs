using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Ball))]
public class BallDrawer : MonoBehaviour
{
    private Ball _ball;

    [SerializeField] private TextMeshPro _text;

    private void Awake()
    {
        _ball = GetComponent<Ball>();
    }

    private void Start()
    {
        UpdateLevel(_ball.Level);
    }

    private void UpdateLevel(int level)
    {
        _text.text = Mathf.Pow(2, level).ToString();
    }

    private void OnEnable()
    {
        _ball.LevelUpdated += UpdateLevel;
    }

    private void OnDisable()
    {
        _ball.LevelUpdated -= UpdateLevel;
    }
}
