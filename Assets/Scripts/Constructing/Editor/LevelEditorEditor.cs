using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelEditor))]
public class LevelEditorEditor : Editor
{
    private LevelEditor _target;

    private void Awake()
    {
        _target = (LevelEditor)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(_target.enabled == false || _target.gameObject.activeInHierarchy == false)
            return;

        if(Application.isPlaying)
        {
            if(GUILayout.Button("Save"))
            {
                string path = EditorUtility.SaveFilePanel("Save path", Application.dataPath, "Map", "asset");

                if(path != null && path.Length > 0)
                {
                    path = "Assets/" + path.Substring(Application.dataPath.Length);
                    SavedMap savedMap = _target.Save();
                    AssetDatabase.CreateAsset(savedMap, path);
                }
            }

            if(GUILayout.Button("Load"))
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
}
