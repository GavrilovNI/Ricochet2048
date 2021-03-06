using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Brick))]
public class BrickDrawer : MonoBehaviour
{
    private Brick _brick;

    [SerializeField] private TextMeshPro _text;

    private void Awake()
    {
        _brick = GetComponent<Brick>();
    }

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        _text.text = Mathf.Pow(2, _brick.Level).ToString();
    }

    private void OnLevelUpdated(Level _)
    {
        UpdateText();
    }

    private void OnEnable()
    {
        _brick.LevelUpdated += OnLevelUpdated;
    }

    private void OnDisable()
    {
        _brick.LevelUpdated -= OnLevelUpdated;
    }

    private void OnDrawGizmos()
    {
        if (_text == null)
            return;

        bool brickIsNull = _brick == null;
        if(brickIsNull)
            _brick = GetComponent<Brick>();
        UpdateText();
        if (brickIsNull)
            _brick = null;
    }
}
