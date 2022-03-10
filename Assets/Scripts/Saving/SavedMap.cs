using UnityEngine;

[CreateAssetMenu(fileName = "MapSave", menuName = "Ricochet/MapSave", order = 2)]
public class SavedMap : ScriptableObject
{
    public FieldSettings LevelSettings;
    public SavedLevel SavedLevel;

    public SavedMap()
    {
        LevelSettings = new FieldSettings();
        SavedLevel = new SavedLevel();
    }
}
