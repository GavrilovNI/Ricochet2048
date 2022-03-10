using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChooser : MonoBehaviour
{
    [SerializeField] private MapLoader _mapLoader;
    [SerializeField] private List<SavedMap> _levels;

    public int LevelsCount => _levels.Count;
    public int LastLevelLoaded { get; private set; } = -1;

    public void StartLevel(int index)
    {
        if(index < 0 || index >= _levels.Count)
            throw new System.ArgumentOutOfRangeException(nameof(index));

        SavedMap savedMap = _levels[index];
        _mapLoader.Load(savedMap);
        _mapLoader.StartMap();
        LastLevelLoaded = index;
    }

    [ContextMenu("Start first level")]
    public void StartFirstLevel()
    {
        _mapLoader.ClearMap();
        StartLevel(0);
    }

    public void SkipLevel()
    {
        _mapLoader.ClearMap();
        LastLevelLoaded++;
    }

    public void RestartLastLoadedLevel()
    {
        _mapLoader.ClearMap();
        StartLevel(LastLevelLoaded);
    }

    public void StartNextLevel()
    {
        _mapLoader.ClearMap();
        StartLevel(LastLevelLoaded + 1);
    }
}
