using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Map))]
[RequireComponent(typeof(MapField))]
public class MapLoader : MonoBehaviour
{
    private Map _map;
    private MapField _mapField;

    private void Awake()
    {
        _map = GetComponent<Map>();
        _mapField = GetComponent<MapField>();
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

    public void Load(SavedMap map)
    {
        _map.Bricks.Clear();
        _mapField.Settings = map.LevelSettings;
        _mapField.Construct();
        _map.MapSettings = map.LevelSettings.MapSettings;

        using(var empEnumerator = map.SavedLevel.GetBricksEnumerator())
        {
            while(empEnumerator.MoveNext())
            {
                var current = empEnumerator.Current;
                SpawnBrick(map.SavedLevel.BrickModels, current.BrickType, current.Position);
            }
        }
    }
}
