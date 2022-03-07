using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TransformExtensions;
using VectorExtensions;

[RequireComponent(typeof(Map))]
[RequireComponent(typeof(MapField))]
public class MapEditor : MonoBehaviour
{
    private Map _map;
    private MapField _field;

    private Transform _editorParent;
    private Transform _brickButtonsParent;

    [SerializeField] private Camera _camera;
    [SerializeField] private CreateBrickButton _createBrickButtonPrefab;
    [SerializeField] private BrickLeveler _brickLevelerPrefab;
    [SerializeField] private ChangeSizeButtons _changeSizeButtonsPrefab;

    private MapSettings Settings => _field.Settings;

    private Dictionary<Vector2Int, BrickButton> _brickButtons = new Dictionary<Vector2Int, BrickButton>();
    private List<ChangeSizeButtons> _changeSizeButtons = new List<ChangeSizeButtons>();

    public float ChangeSizeButtonsWidth => Mathf.Max(1f, Mathf.Max(Settings.MapSize.x, Settings.MapSize.y) / 10f);

    private void Awake()
    {
        _map = GetComponent<Map>();
        _field = GetComponent<MapField>();

        _editorParent = new GameObject("Editing").transform;
        _editorParent.SetParent(transform);
        _brickButtonsParent = new GameObject("BrickButtons").transform;
        _brickButtonsParent.SetParent(_editorParent);
    }

    private void Start()
    {
        CreateMissingBrickButtons();
        UpdateCamera();
        CreateChangeSizeButtons();
    }

    private void OnEnable()
    {
        _editorParent.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _editorParent.gameObject.SetActive(false);
    }

    public MapSave Save()
    {
        MapSave result = new MapSave();
        foreach(var button in _brickButtons)
        {
            if(button.Value is BrickLeveler leveler)
                result.Set(button.Key, new BrickSave(leveler.BrickModel.Index, leveler.Level));
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
        UpdateButtons();
        UpdateChangeSizeButtons();
        _field.ConstructDefault();
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
        UpdateButtons();
        UpdateChangeSizeButtons();
        _field.ConstructDefault();
    }

    private void RemoveButtonsInRange(int fromX, int fromY, int toX, int toY)
    {
        for(int y = fromY; y < toY; y++)
        {
            for(int x = fromX; x < toX; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                Destroy(_brickButtons[position].gameObject);
                _brickButtons.Remove(position);
            }
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
            var buttons = Instantiate(_changeSizeButtonsPrefab, _editorParent);
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
    private void UpdateButtons()
    {
        foreach(var button in _brickButtons)
        {
            SetupButton(button.Value, button.Key);
        }
    }
    private BrickButton CreateButton(Vector2Int position, BrickButton prefab)
    {
        if(_brickButtons.ContainsKey(position))
            throw new System.InvalidOperationException("Position already contains button");

        Vector3 position3D = _field.GetBrickPosition(position);
        BrickButton brickButton = GameObject.Instantiate(prefab);
        SetupButton(brickButton, position);

        _brickButtons.Add(position, brickButton);

        brickButton.Switch.AddListener(() => SwitchButton(position));

        return brickButton;
    }
    private void SetupButton(BrickButton brickButton, Vector2Int position)
    {
        Vector3 position3D = _field.GetBrickPosition(position);
        brickButton.transform.position = position3D;
        brickButton.transform.rotation = Quaternion.identity;
        brickButton.transform.SetParent(_brickButtonsParent, true);
        brickButton.transform.SetGlobalScale(Settings.BrickSize);
    }
    private void SwitchButton(Vector2Int position)
    {
        Component button = _brickButtons[position];
        bool isLeveler = button is BrickLeveler;
        Button2D.Destroy(button.gameObject);
        _brickButtons.Remove(position);

        BrickButton prefab = isLeveler ? (BrickButton)_createBrickButtonPrefab : _brickLevelerPrefab;
        CreateButton(position, prefab);
    }
}
