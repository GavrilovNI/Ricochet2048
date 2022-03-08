using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedLevel
{
    [System.Serializable]
    public class BrickInfo
    {
        public Vector2Int Position;
        public BrickType BrickType;

        public BrickInfo()
        {

        }

        public BrickInfo(Vector2Int position, BrickType brickType)
        {
            Position = position;
            BrickType = brickType;
        }
    }


    [SerializeField] private BrickModels _brickModels;
    [SerializeField] private List<BrickInfo> _bricks = new List<BrickInfo>();

    public BrickModels BrickModels
    {
        get => _brickModels;
        set => _brickModels = value;
    }

    public SavedLevel()
    {

    }

    public bool Contains(Vector2Int position)
    {
        return GetIndex(position) != -1;
    }
    private int GetIndex(Vector2Int position)
    {
        return _bricks.FindIndex(x => x.Position == position);
    }

    public void Set(Vector2Int position, BrickType brick)
    {
        int index = GetIndex(position);
        bool contains = index != -1;
        if(contains)
        {
            _bricks.RemoveAt(index);
            if(brick != null)
                _bricks.Insert(index, new BrickInfo(position, brick));
        }
        else
        {
            if(brick != null)
                _bricks.Add(new BrickInfo(position, brick));
        }
    }

    public List<BrickInfo>.Enumerator GetBricksEnumerator()
    {
        return _bricks.GetEnumerator();
    }
}
