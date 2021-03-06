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
        UpdateText();
    }

    private void UpdateText()
    {
        _text.text = Mathf.Pow(2, _ball.Level).ToString();
    }

    private void OnLevelUpdated(Ball ball, Level level)
    {
        UpdateText();
    }

    private void OnEnable()
    {
        _ball.LevelUpdated += OnLevelUpdated;
    }

    private void OnDisable()
    {
        _ball.LevelUpdated -= OnLevelUpdated;
    }

    private void OnDrawGizmos()
    {
        if (_text == null)
            return;

        bool ballIsNull = _ball == null;
        if (ballIsNull)
            _ball = GetComponent<Ball>();
        UpdateText();
        if (ballIsNull)
            _ball = null;
    }
}
