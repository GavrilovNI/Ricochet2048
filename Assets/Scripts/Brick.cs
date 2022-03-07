using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public event System.Action<Level> LevelUpdated;

    [SerializeField] private Level _level;
    [SerializeField] private BrickModel _model;
    private Transform _spawnedModel;

    public BrickModel BrickModel => _model;

    public Level Level
    {
        get => _level;
        set
        {
            _level = value;
            LevelUpdated(_level);
        }
    }

    private void Awake()
    {
        _model.ModelUpdated += UpdateModel;
    }

    private void Start()
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
    }
}
