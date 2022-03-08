using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapLoader))]
public class MapLoaderEditor : Editor
{
    private MapLoader _target;

    private void Awake()
    {
        _target = (MapLoader)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(_target.enabled == false || _target.gameObject.active == false)
            return;

        if(Application.isPlaying && GUILayout.Button("Load"))
        {
            string path = EditorUtility.OpenFilePanel("Save path", Application.dataPath, "asset");

            if(path.Length > 0)
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
                SavedMap savedMap = (SavedMap)AssetDatabase.LoadAssetAtPath(path, typeof(SavedMap));
                if(savedMap == null)
                    Debug.Log("Map file not found.");
                else
                    _target.Load(savedMap);
            }
        }
    }
}
