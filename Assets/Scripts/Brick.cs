using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public event System.Action<Level> LevelUpdated;

    [SerializeField] private Level _level;
    [SerializeField] private GameObject _modelPrefab;
    private Transform _spawnedModel;

    public Level Level
    {
        get => _level;
        set
        {
            _level = value;
            LevelUpdated(_level);
        }
    }

    public void SetModel(BrickModel model)
    {
        if(_spawnedModel != null)
            GameObject.Destroy(_spawnedModel.gameObject);
        if(model.IsValid)
        {
            _spawnedModel = model.Spawn();
            _spawnedModel.SetParent(transform, false);
        }
    }
}
