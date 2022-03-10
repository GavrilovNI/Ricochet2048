using TMPro;
using UnityEngine;
using UnityEngine.Events;
using TransformExtensions;

public class BrickLeveler : BrickButton
{
    public UnityEvent PickModel;
    public UnityEvent Increasing;
    public UnityEvent Decreasing;

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
    [SerializeField] private Transform _buttonsParent;

    [SerializeField] private BrickModel _model;
    public int ModelIndex => _model.Index;
    private Transform _spawnedModel = null;
    private Vector2 _scale = Vector2.one;
    public override Vector2 Scale => _scale;

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

    public override void SetScale(Vector2 scale)
    {
        if(_spawnedModel == null)
        {
            _scale = scale;
        }
        else
        {
            _spawnedModel.SetGlobalScale(scale);

            float buttonsRatio = _buttonsParent.transform.lossyScale.x / _buttonsParent.transform.lossyScale.y;

            Vector2 buttonsParentScale;
            if(scale.x / buttonsRatio <= scale.y)
                buttonsParentScale = new Vector2(scale.x, scale.x / buttonsRatio);
            else
                buttonsParentScale = new Vector2(scale.y * buttonsRatio, scale.y);

            _buttonsParent.SetGlobalScale(buttonsParentScale);
        }
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
            SetScale(_scale);
        }
    }

    private void UpdateLevelText()
    {
        _levelText.text = ((int)_level).ToString();
    }

    public void Increase()
    {
        Increasing?.Invoke();
        Level = (Level)(Level + 1);
    }

    public void Decrease()
    {
        Decreasing?.Invoke();
        int level = Level - 1;
        if(level >= 0)
            Level = (Level)level;
    }
}
