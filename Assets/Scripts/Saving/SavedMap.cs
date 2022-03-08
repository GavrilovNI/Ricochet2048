using UnityEngine;

[CreateAssetMenu(fileName = "MapSave", menuName = "Ricochet/MapSave", order = 2)]
public class SavedMap : ScriptableObject
{
    public LevelSettings LevelSettings;
    public SavedLevel SavedLevel;

    public SavedMap()
    {
        LevelSettings = new LevelSettings();
        SavedLevel = new SavedLevel();
    }
}
