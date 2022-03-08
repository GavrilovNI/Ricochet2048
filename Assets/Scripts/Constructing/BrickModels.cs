using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "BrickModels", menuName = "Ricochet/BrickModels", order = 1)]
public class BrickModels : ScriptableObject
{
    [SerializeField] private List<GameObject> _models = new List<GameObject>();

    public int Count => _models.Count;
    public GameObject this[int index] => _models[index];
}
