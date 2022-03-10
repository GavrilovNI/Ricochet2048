using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TransformExtensions;
using VectorExtensions;
using System.Linq;

[RequireComponent(typeof(Map))]
[RequireComponent(typeof(MapField))]
public class LevelEditor : MonoBehaviour
{
    private Map _map;
    private MapField _field;

    private Transform _editorParent;
    private Transform _brickButtonsParent;
    private Transform _changeSizeButtonsParent;

    [SerializeField] private Camera _camera;
    [SerializeField] private CreateBrickButton _createBrickButtonPrefab;
    [SerializeField] private BrickLeveler _brickLevelerPrefab;
    [SerializeField] private ChangeSizeButtons _changeSizeButtonsPrefab;
    [SerializeField] private BrickModels _brickModels;

    [SerializeField] private BrickModelPicker _modelPickerPrefab;
    private BrickModelPicker _modelPicker;
    private Vector2Int _currentModelPickingBrick = -Vector2Int.one;

    private LevelSettings Settings => _field.Settings;
    private Dictionary<Vector2Int, BrickButton> _brickButtons = new Dictionary<Vector2Int, BrickButton>();
    private List<ChangeSizeButtons> _changeSizeButtons = new List<ChangeSizeButtons>();

    public float ChangeSizeButtonsWidth => Mathf.Max(1f, Mathf.Max(Settings.MapSize.x, Settings.MapSize.y) / 10f);

    private void Awake()
    {
        _map = GetComponent<Map>();
        _field = GetComponent<MapField>();
    }

    private void Start()
    {
        _editorParent = new GameObject("Editing").transform;
        _editorParent.SetParent(transform);
        _editorParent.gameObject.SetActive(enabled);
        _brickButtonsParent = new GameObject("BrickButtons").transform;
        _brickButtonsParent.SetParent(_editorParent);
        _changeSizeButtonsParent = new GameObject("ChangeSizeButtons").transform;
        _changeSizeButtonsParent.SetParent(_editorParent);

        _modelPicker = Instantiate(_modelPickerPrefab, _editorParent);
        _modelPicker.ModelPicked.AddListener(OnModelPicked);


        HideModelPicker();
        CreateMissingBrickButtons();
        CreateChangeSizeButtons();
        UpdateCamera();
    }

    private void OnEnable()
    {
        if(_editorParent != null)
            _editorParent.gameObject.SetActive(true);
        RemoveInvalidButtons();
        CreateMissingBrickButtons();
        UpdateButtonSizesAndPositions();
        UpdateChangeSizeButtons();
        _field.ReConstruct();
    }

    private void OnDisable()
    {
        _editorParent.gameObject.SetActive(false);
    }

    public void Load(SavedMap map)
    {
        _map.Bricks.Clear();
        LevelSettings levelSettings = new LevelSettings(map.LevelSettings);
        _field.Construct(levelSettings);
        _map.MapSettings = levelSettings.MapSettings;
        RemoveAllButtons();
        _brickModels = map.SavedLevel.BrickModels;
        using(var empEnumerator = map.SavedLevel.GetBricksEnumerator())
        {
            while(empEnumerator.MoveNext())
            {
                var current = empEnumerator.Current;
                BrickLeveler button = CreateButton(current.Position, _brickLevelerPrefab);
                button.Level = current.BrickType.Level;
                button.SetModel(new BrickModel(_brickModels, current.BrickType.BrickModelIndex));
            }
        }
        CreateMissingBrickButtons();
        UpdateChangeSizeButtons();
    }

    public SavedMap Save()
    {
        SavedMap result = ScriptableObject.CreateInstance<SavedMap>();
        result.LevelSettings = new LevelSettings(Settings);
        result.SavedLevel = new SavedLevel();
        result.SavedLevel.BrickModels = _brickModels;
        foreach(var button in _brickButtons)
        {
            if(button.Value is BrickLeveler leveler)
                result.SavedLevel.Set(button.Key, new BrickType(leveler.ModelIndex, leveler.Level));
        }
        return result;
    }

    private void UpdateCamera()
    {
        float minCameraWidth = Settings.MapSize.x + 2 * ChangeSizeButtonsWidth;
        float minCameraHeight = Settings.MapSize.y + 2 * ChangeSizeButtonsWidth;

        float cameraSize;
        if(minCameraWidth / _camera.aspect < minCameraHeight)
            cameraSize = minCameraHeight * _camera.aspect;
        else
            cameraSize = minCameraWidth;

        _camera.orthographicSize = cameraSize;
        _camera.transform.position = _field.MapBounds.center.XY().ToV3(_camera.transform.position.z);
    }

    private void IncreaseSize(ChangeSizeButtons.ChangeDirection direction)
    {
        switch(direction)
        {
            case ChangeSizeButtons.ChangeDirection.Right:
            case ChangeSizeButtons.ChangeDirection.Left:
                Settings.MapSizeInBricks += Vector2Int.right;
                break;
            case ChangeSizeButtons.ChangeDirection.Top:
            case ChangeSizeButtons.ChangeDirection.Bottom:
                Settings.MapSizeInBricks += Vector2Int.up;
                break;
        }
        switch(direction)
        {
            case ChangeSizeButtons.ChangeDirection.Left:
                MoveAllButtons(Vector2Int.right);
                break;
            case ChangeSizeButtons.ChangeDirection.Bottom:
                MoveAllButtons(Vector2Int.up);
                break;
        }

        CreateMissingBrickButtons();
        UpdateOnBrickSizeChanged();
        HideModelPicker();
    }
    private void DecreaseSize(ChangeSizeButtons.ChangeDirection direction)
    {
        switch(direction)
        {
            case ChangeSizeButtons.ChangeDirection.Right:
                if(Settings.MapSizeInBricks.x <= 1)
                    return;
                Settings.MapSizeInBricks -= Vector2Int.right;
                RemoveButtonsInRange(Settings.MapSizeInBricks.x, 0,
                                     Settings.MapSizeInBricks.x + 1, Settings.MapSizeInBricks.y);
                break;
            case ChangeSizeButtons.ChangeDirection.Top:
                if(Settings.MapSizeInBricks.y <= 1)
                    return;
                Settings.MapSizeInBricks -= Vector2Int.up;
                RemoveButtonsInRange(0, Settings.MapSizeInBricks.y,
                                     Settings.MapSizeInBricks.x, Settings.MapSizeInBricks.y + 1);
                break;
            case ChangeSizeButtons.ChangeDirection.Left:
                if(Settings.MapSizeInBricks.x <= 1)
                    return;
                Settings.MapSizeInBricks -= Vector2Int.right;
                RemoveButtonsInRange(0, 0,
                                     1, Settings.MapSizeInBricks.y);
                MoveAllButtons(Vector2Int.left);
                break;
            case ChangeSizeButtons.ChangeDirection.Bottom:
                if(Settings.MapSizeInBricks.y <= 1)
                    return;
                Settings.MapSizeInBricks -= Vector2Int.up;
                RemoveButtonsInRange(0, 0,
                                     Settings.MapSizeInBricks.x, 1);
                MoveAllButtons(Vector2Int.down);
                break;
        }
        UpdateOnBrickSizeChanged();
        HideModelPicker();
    }

    private void RemoveInvalidButtons()
    {
        foreach(var button in _brickButtons.ToList())
        {
            if(_field.IsInside(button.Key) == false)
                _brickButtons.Remove(button.Key);
        }
    }
    private void RemoveButton(Vector2Int position)
    {
        Destroy(_brickButtons[position].gameObject);
        _brickButtons.Remove(position);
    }
    private void RemoveButtonsInRange(int fromX, int fromY, int toX, int toY)
    {
        for(int y = fromY; y < toY; y++)
        {
            for(int x = fromX; x < toX; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                RemoveButton(position);
            }
        }
    }
    private void RemoveAllButtons()
    {
        foreach(var button in _brickButtons.ToList())
        {
            RemoveButton(button.Key);
        }
    }
    private void MoveAllButtons(Vector2Int delta)
    {
        Dictionary<Vector2Int, BrickButton> newButtons = new Dictionary<Vector2Int, BrickButton>();
        foreach(var button in _brickButtons)
        {
            Vector2Int newPosition = button.Key + delta;
            BrickButton brickButton = button.Value;
            brickButton.transform.position = _field.GetBrickPosition(newPosition);
            newButtons.Add(newPosition, brickButton);
            brickButton.Switch.RemoveAllListeners();
            brickButton.Switch.AddListener(() => SwitchButton(newPosition));
        }
        _brickButtons = newButtons;
    }

    private void CreateChangeSizeButtons()
    {
        foreach(var direction in ChangeSizeButtons.Directions)
        {
            var buttons = Instantiate(_changeSizeButtonsPrefab, _changeSizeButtonsParent);
            buttons.Direction = direction;
            buttons.Increase.AddListener(IncreaseSize);
            buttons.Decrease.AddListener(DecreaseSize);
            _changeSizeButtons.Add(buttons);
        }

        UpdateChangeSizeButtons();
    }
    private void UpdateChangeSizeButtons()
    {
        Bounds bounds = _field.MapBounds;

        foreach(var buttons in _changeSizeButtons)
        {
            float halfWidth = ChangeSizeButtonsWidth / 2f;
            switch(buttons.Direction)
            {
                case ChangeSizeButtons.ChangeDirection.Top:
                    buttons.transform.localScale = new Vector3(ChangeSizeButtonsWidth, bounds.size.x);
                    buttons.transform.position = new Vector3(bounds.center.x, bounds.max.y + halfWidth);
                    break;
                case ChangeSizeButtons.ChangeDirection.Bottom:
                    buttons.transform.localScale = new Vector3(ChangeSizeButtonsWidth, bounds.size.x);
                    buttons.transform.position = new Vector3(bounds.center.x, bounds.min.y - halfWidth);
                    break;
                case ChangeSizeButtons.ChangeDirection.Left:
                    buttons.transform.localScale = new Vector3(ChangeSizeButtonsWidth, bounds.size.y);
                    buttons.transform.position = new Vector3(bounds.min.x - halfWidth, bounds.center.y);
                    break;
                case ChangeSizeButtons.ChangeDirection.Right:
                    buttons.transform.localScale = new Vector3(ChangeSizeButtonsWidth, bounds.size.y);
                    buttons.transform.position = new Vector3(bounds.max.x + halfWidth, bounds.center.y);
                    break;
            }
        }
    }
    
    private void CreateMissingBrickButtons()
    {
        for(int y = 0; y < Settings.MapSizeInBricks.y; y++)
        {
            for(int x = 0; x < Settings.MapSizeInBricks.x; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if(_brickButtons.ContainsKey(position) == false)
                    CreateButton(position, _createBrickButtonPrefab);
            }
        }
    }
    private void UpdateButtonSizesAndPositions()
    {
        foreach(var button in _brickButtons)
        {
            SetupButton(button.Value, button.Key);
        }
    }
    private void UpdateOnBrickSizeChanged()
    {
        UpdateButtonSizesAndPositions();
        UpdateChangeSizeButtons();
        _field.ReConstruct();
    }

    private void HideModelPicker()
    {
        _currentModelPickingBrick = -Vector2Int.one;
        _modelPicker.gameObject.SetActive(false);
    }

    private void ShowOrHideModelPicker(Vector2Int position)
    {
        if(_currentModelPickingBrick == position)
        {
            HideModelPicker();
        }
        else
        {
            Vector3 pickModelPosition = _field.GetBrickPosition(position) +
                                        Settings.BrickSize.Invert(Coordinate2D.Y).ToV3() / 2f +
                                        Vector3.back; // for beeing above all brick buttons
            _modelPicker.transform.position = pickModelPosition;
            _modelPicker.gameObject.SetActive(true);
            _currentModelPickingBrick = position;
            _modelPicker.BrickModels = _brickModels;
            _modelPicker.BrickSize = Settings.BrickSize;
        }
    }

    private void OnModelPicked(int modelIndex)
    {
        if(_currentModelPickingBrick == -Vector2Int.one)
            throw new System.InvalidOperationException("Can't set model: brick position not found.");

        if(_brickButtons[_currentModelPickingBrick] is BrickLeveler leveler)
            leveler.SetModel(new BrickModel(_brickModels, modelIndex));
        else
            throw new System.InvalidOperationException("Can't set model: brick not found.");

        HideModelPicker();
    }

    private T CreateButton<T>(Vector2Int position, T prefab) where T : BrickButton
    {
        if(_brickButtons.ContainsKey(position))
            throw new System.InvalidOperationException("Position already contains button");

        Vector3 position3D = _field.GetBrickPosition(position);
        T brickButton = GameObject.Instantiate(prefab);
        SetupButton(brickButton, position);

        _brickButtons.Add(position, brickButton);

        brickButton.Switch.AddListener(() => {
            HideModelPicker();
            SwitchButton(position);
        });

        if(brickButton is BrickLeveler leveler)
        {
            leveler.SetModel(new BrickModel(_brickModels, 0));
            leveler.PickModel.AddListener(() => ShowOrHideModelPicker(position));
            leveler.Increasing.AddListener(() => HideModelPicker());
            leveler.Decreasing.AddListener(() => HideModelPicker());
        }

        return brickButton;
    }
    private void SetupButton(BrickButton brickButton, Vector2Int position)
    {
        Vector3 position3D = _field.GetBrickPosition(position);
        brickButton.transform.position = position3D;
        brickButton.transform.rotation = Quaternion.identity;
        brickButton.transform.SetParent(_brickButtonsParent, true);
        brickButton.SetScale(Settings.BrickSize);
        brickButton.name = brickButton.GetType().Name + " " + position.ToString();
    }
    private void SwitchButton(Vector2Int position)
    {
        BrickButton oldButton = _brickButtons[position];
        _brickButtons.Remove(position);

        if(oldButton.Scale != Settings.BrickSize)
            UpdateOnBrickSizeChanged();

        bool isLeveler = oldButton is BrickLeveler;
        Button2D.Destroy(oldButton.gameObject);

        BrickButton prefab = isLeveler ? (BrickButton)_createBrickButtonPrefab : _brickLevelerPrefab;
        CreateButton(position, prefab);
    }
}
