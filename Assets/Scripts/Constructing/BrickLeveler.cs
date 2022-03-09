using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BrickLeveler : BrickButton
{
    public UnityEvent PickModel;

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
    [SerializeField] private Button2D _pickModelButton;

    [SerializeField] private BrickModel _model;
    public int ModelIndex => _model.Index;
    private Transform _spawnedModel = null;

    public void Awake()
    {
        _removeButton.MouseClick.AddListener((mouseButton) =>
        {
            if(mouseButton == MouseButton.Left)
                Switch?.Invoke();
        });
        _pickModelButton.MouseClick.AddListener((mouseButton) =>
        {
            if(mouseButton == MouseButton.Left)
                PickModel?.Invoke();
        });
        UpdateLevelText();
    }

    public void Start()
    {
        if(_spawnedModel == null)
            SetModel(_model);
    }

    public void SetModel(BrickModel model)
    {
        if(_spawnedModel != null)
            GameObject.Destroy(_spawnedModel.gameObject);
        _model = model;
        if(model.IsValid)
        {
            _spawnedModel = model.Spawn();
            _spawnedModel.SetParent(transform, false);
            foreach(var collider in _spawnedModel.GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
        }
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
