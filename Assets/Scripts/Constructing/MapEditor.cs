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

    private Transform _emptiesParent;
    private Transform _brickLevelersParent;

    [SerializeField] private Button2D _createBrickButtonPrefab;
    [SerializeField] private BrickLeveler _brickLevelerPrefab;

    private Dictionary<Vector2Int, Button2D> _empties = new Dictionary<Vector2Int, Button2D>();
    private Dictionary<Vector2Int, BrickLeveler> _brickLevelers = new Dictionary<Vector2Int, BrickLeveler>();

    private void Awake()
    {
        _map = GetComponent<Map>();
        _field = GetComponent<MapField>();

        _emptiesParent = new GameObject("Empties").transform;
        _emptiesParent.SetParent(transform);
        _brickLevelersParent = new GameObject("Brick Levelers").transform;
        _brickLevelersParent.SetParent(transform);
    }

    private void Start()
    {
        CreateAllCreateBrickButtons();
    }
    
    private void CreateAllCreateBrickButtons()
    {
        for(int y = 0; y < _field.MapSizeInBricks.y; y++)
        {
            for(int x = 0; x < _field.MapSizeInBricks.x; x++)
            {
                CreateCreateBrickButton(new Vector2Int(x, y));
            }
        }
    }

    private Button2D CreateCreateBrickButton(Vector2Int position)
    {
        Vector3 position3D = _field.GetBrickPosition(position);
        Button2D addBrickButton = GameObject.Instantiate(_createBrickButtonPrefab);
        addBrickButton.transform.position = position3D;
        addBrickButton.transform.rotation = Quaternion.identity;
        addBrickButton.transform.SetParent(_emptiesParent, true);
        addBrickButton.transform.SetGlobalScale(_field.BrickSize);

        _empties.Add(position, addBrickButton);

        addBrickButton.MouseDown.AddListener(() => OnAddBrickButtonPressed(position));

        return addBrickButton;
    }

    private BrickLeveler CreateBrickLevelerButton(Vector2Int position)
    {
        Vector3 position3D = _field.GetBrickPosition(position);
        BrickLeveler brickLeveler = GameObject.Instantiate(_brickLevelerPrefab);
        brickLeveler.transform.position = position3D;
        brickLeveler.transform.rotation = Quaternion.identity;
        brickLeveler.transform.SetParent(_brickLevelersParent, true);
        brickLeveler.transform.SetGlobalScale(_field.BrickSize);

        _brickLevelers.Add(position, brickLeveler);

        brickLeveler.Remove.AddListener(() => OnBrickLevelerRemoveButtonPressed(position));

        return brickLeveler;
    }

    private void OnAddBrickButtonPressed(Vector2Int position)
    {
        Button2D button = _empties[position];
        _empties.Remove(position);
        Button2D.Destroy(button.gameObject);

        CreateBrickLevelerButton(position);
    }
    private void OnBrickLevelerRemoveButtonPressed(Vector2Int position)
    {
        BrickLeveler brickLeveler = _brickLevelers[position];
        _brickLevelers.Remove(position);
        GameObject.Destroy(brickLeveler.gameObject);

        CreateCreateBrickButton(position);
    }
}
