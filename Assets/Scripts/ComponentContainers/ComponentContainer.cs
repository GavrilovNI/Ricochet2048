using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentContainer<T> : MonoBehaviour where T : Component
{
    public event System.Action<T> Added;
    public event System.Action<T> Removed;

    public int Count => _component.Count;

    [SerializeField] private bool _addChildrenOnStart = true;

    private HashSet<T> _component = new HashSet<T>();

    private void Start()
    {
        if (_addChildrenOnStart)
            AddAllChildren();
    }

    private void AddAllChildren()
    {
        T[] components = GetComponentsInChildren<T>();
        foreach (T component in components)
        {
            Add(component);
        }
    }

    public bool Add(T component)
    {
        bool added = _component.Add(component);
        if(added)
        {
            component.transform.SetParent(transform, true);
            Added?.Invoke(component);
        }
        return added;
    }

    public bool Remove(T component)
    {
        bool removed =_component.Remove(component);
        if(removed)
        {
            component.transform.SetParent(null, true);
            Removed?.Invoke(component);
        }
        return removed;
    }

    public bool Contains(T component)
    {
        return _component.Contains(component);
    }
}
