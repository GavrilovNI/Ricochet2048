using System;
using UnityEngine;

[Serializable]
public class BrickModel
{
    [SerializeField] private BrickModels _brickModels;
    [SerializeField, Min(-1)] private int _brickModelIndex = -1;

    public BrickModels Models
    {
        get => _brickModels;
        set
        {
            _brickModels = value;
        }
    }
    public int Index
    {
        get
        {
            if(_brickModelIndex >= _brickModels.Count || _brickModelIndex < -1)
                _brickModelIndex = -1;
            return _brickModelIndex;
        }
        set
        {
            if(value < -1 || value >= _brickModels.Count)
                throw new ArgumentOutOfRangeException(nameof(Index));
            else
                _brickModelIndex = value;
        }
    }

    public bool IsValid => Index >= 0;

    public BrickModel()
    {

    }

    public BrickModel(BrickModels models, int index)
    {
        _brickModels = models;
        Index = index;
    }

    public Transform Spawn()
    {
        if(IsValid == false)
            throw new InvalidOperationException("Brick Model is onvalid.");

        Transform spawnedModel = GameObject.Instantiate(_brickModels[Index]).transform;
        spawnedModel.name = "Model";
        spawnedModel.transform.position = Vector3.zero;
        return spawnedModel;
    }
}
