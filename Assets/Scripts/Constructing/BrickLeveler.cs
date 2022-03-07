using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BrickLeveler : BrickButton
{
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

    [SerializeField] private BrickModel _model;
    public BrickModel BrickModel => _model;
    private Transform _spawnedModel;

    public void Awake()
    {
        _removeButton.MouseClick.AddListener(() => Switch?.Invoke());
        UpdateLevelText();
        _model.ModelUpdated += UpdateModel;
    }

    public void Start()
    {
        UpdateModel(_model.Models, _model.Index);
    }

    private void UpdateModel(BrickModels models, int index)
    {
        if(_spawnedModel != null)
            GameObject.Destroy(_spawnedModel.gameObject);
        if(_model.Valid == false)
            return;
        _spawnedModel = _model.Spawn();
        _spawnedModel.SetParent(transform, false);

        var colliders = _spawnedModel.GetComponentsInChildren<Collider2D>();
        foreach(var collider in colliders)
        {
            collider.enabled = false;
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
