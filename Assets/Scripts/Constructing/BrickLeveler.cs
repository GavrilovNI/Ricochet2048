using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BrickLeveler : MonoBehaviour
{
    public UnityEvent Remove;

    private Level _level = new Level();
    public Level Level
    {
        get => _level;
        set
        {
            _level = value;
            UpdateLevelText();
        }
    }

    [SerializeField] private TextMeshPro _levelText;
    [SerializeField] private Button2D _removeButton;

    public void Awake()
    {
        _removeButton.MouseDown.AddListener(() => Remove?.Invoke());
        UpdateLevelText();
    }

    private void UpdateLevelText()
    {
        _levelText.text = ((int)_level).ToString();
    }

    public void Increase()
    {
        Level = (Level)(Level + 1);
    }

    public void Decrease()
    {
        int level = Level - 1;
        if(level >= 0)
            Level = (Level)level;
    }
}
