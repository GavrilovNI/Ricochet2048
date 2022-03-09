using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TransformExtensions;
using VectorExtensions;

public class BrickModelPicker : MonoBehaviour
{
    [SerializeField] private BrickModels _brickModels;
    [SerializeField] private Vector2 _brickSize = Vector2.one;

    [SerializeField] private SpriteRenderer _background;
    [SerializeField, Min(0f)] private float _padding = 0.1f;
    [SerializeField, Min(1)] private int _width = 4;
    [SerializeField] private Button2D _pickableModelPrefab;
    
    public UnityEvent<int> ModelPicked;

    private List<Button2D> _buttons = new List<Button2D>();

    private Vector2 Padding => Vector2.one * _padding;

    public BrickModels BrickModels
    {
        get => _brickModels;
        set
        {
            _brickModels = value;
            SpawnButtons();
        }
    }

    public Vector2 BrickSize
    {
        get => _brickSize;
        set
        {
            if(value.x <= 0 || value.y <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(BrickSize));
            else
            {
                _brickSize = value;
                SpawnButtons();
            }
        }
    }

    private void Start()
    {
        if(_brickModels != null)
            SpawnButtons();
    }

    private void Pick(int modelIndex)
    {
        ModelPicked?.Invoke(modelIndex);
    }

    public void SpawnButtons()
    {
        foreach (var button in _buttons)
            GameObject.Destroy(button.gameObject);
        _buttons.Clear();
        Vector2Int size = new Vector2Int(_brickModels.Count >= _width ? _width : _brickModels.Count,
                                         (_brickModels.Count + _width - 1) / _width);

        _background.transform.SetGlobalScale(size * (_brickSize + Padding) + Padding);
        _background.transform.position = transform.position + _background.transform.lossyScale.Invert(Coordinate3D.Y) / 2f;
        for(int index = 0; index < _brickModels.Count; index++)
        {
            Button2D button = SpawnButton(index, _background.sortingOrder + 1);
            _buttons.Add(button);

            Vector2Int position = new Vector2Int(index % _width, index / _width);

            button.transform.position = transform.position + (position * (_brickSize + Padding) + Padding + _brickSize / 2f).Invert(Coordinate2D.Y).ToV3();
            button.transform.SetParent(transform, true);
        }

    }

    private Button2D SpawnButton(int modelIndex, int rendererLayer = 0)
    {
        BrickModel model = new BrickModel(_brickModels, modelIndex);
        if(model.IsValid == false)
            throw new System.ArgumentOutOfRangeException(nameof(modelIndex));
        Button2D button = Instantiate(_pickableModelPrefab);
        Transform spawndeModel = model.Spawn();
        
        var modelRenderer = spawndeModel.GetComponent<SpriteRenderer>();
        if(modelRenderer != null)
            modelRenderer.sortingOrder = rendererLayer;

        spawndeModel.SetParent(button.transform, false);
        spawndeModel.transform.localPosition = Vector3.zero;
        foreach(var collider in spawndeModel.GetComponentsInChildren<Collider2D>())
            collider.enabled = false;

        button.transform.SetGlobalScale(_brickSize);

        button.MouseClick.AddListener((mouseButton) =>
        {
            if(mouseButton == MouseButton.Left)
                Pick(model.Index);
        });

        return button;
    }



}
