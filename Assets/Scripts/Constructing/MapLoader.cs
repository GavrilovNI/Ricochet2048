using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Map))]
[RequireComponent(typeof(MapField))]
public class MapLoader : MonoBehaviour
{
    public UnityEvent LevelCompleted;
    public UnityEvent LevelFailed;

    private Map _map;
    private MapField _mapField;

    [SerializeField] private bool _clearMapOnStart = false;

    private void Awake()
    {
        _map = GetComponent<Map>();
        _mapField = GetComponent<MapField>();
    }

    private void OnlevelCompleted() => LevelCompleted?.Invoke();
    private void OnlevelFailed() => LevelFailed?.Invoke();

    private void OnEnable()
    {
        _map.Won += OnlevelCompleted;
        _map.Lost += OnlevelFailed;
    }

    private void OnDisable()
    {
        _map.Won -= OnlevelCompleted;
        _map.Lost -= OnlevelFailed;
    }

    private void Start()
    {
        if(_clearMapOnStart)
        {
            if(_map.State == Map.GameState.Playing)
                ClearMap();
            _mapField.Hide();
        }
    }

    private void SpawnBrick(BrickModels models, BrickType brickType, Vector2Int position)
    {
        Vector3 position3D = _mapField.GetBrickPosition(position);
        Brick brick = Instantiate(_mapField.BrickPrefab);
        brick.transform.position = position3D;
        brick.transform.rotation = Quaternion.identity;

        brick.SetModel(new BrickModel(models, brickType.BrickModelIndex));
        brick.Level = brickType.Level;

        _map.Bricks.Add(brick);
        
    }

    public void ClearMap()
    {
        _map.Clear();
        _mapField.Hide();
    }

    public void Load(SavedMap map)
    {
        if(_map.State == Map.GameState.Playing)
            throw new System.InvalidOperationException($"{nameof(Map.GameState)} can't be {nameof(Map.GameState.Playing)} when loading new Level.");

        _map.Clear();
        _map.MapSettings = map.LevelSettings.MapSettings;
        _mapField.Construct(map.LevelSettings);

        using(var empEnumerator = map.SavedLevel.GetBricksEnumerator())
        {
            while(empEnumerator.MoveNext())
            {
                var current = empEnumerator.Current;
                SpawnBrick(map.SavedLevel.BrickModels, current.BrickType, current.Position);
            }
        }
    }

    public void StartMap()
    {
        if(_map.State != Map.GameState.Cleared)
            throw new System.InvalidOperationException("Map is not ready to start.");

        _map.StartGame();
        _mapField.Show();
        _mapField.SpawnBall();
    }
}
