using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentContainer<T> : MonoBehaviour where T : Component
{
    public event System.Action<T> Added;
    public event System.Action<T> Removed;

    public int Count => _components.Count;

    [SerializeField] private bool _addChildrenOnStart = true;

    private HashSet<T> _components = new HashSet<T>();

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
        bool added = _components.Add(component);
        if(added)
        {
            component.transform.SetParent(transform, true);
            Added?.Invoke(component);
        }
        return added;
    }

    public bool Remove(T component)
    {
        bool removed =_components.Remove(component);
        if(removed)
        {
            component.transform.SetParent(null, true);
            Removed?.Invoke(component);
        }
        return removed;
    }

    public bool Contains(T component)
    {
        return _components.Contains(component);
    }
    
    public void Clear()
    {
        foreach(var component in _components)
        {
            component.transform.SetParent(null, true);
            Removed?.Invoke(component);
        }
        _components.Clear();
    }

    public HashSet<T>.Enumerator GetEnumerator() => _components.GetEnumerator();
}
