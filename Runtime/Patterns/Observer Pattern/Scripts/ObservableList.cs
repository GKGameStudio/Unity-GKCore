
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObservableListOperation : byte
{
    /// <summary>
    /// An item is added to the collection.
    /// </summary>
    Add,
    /// <summary>
    /// An item is inserted into the collection.
    /// </summary>
    Insert,
    /// <summary>
    /// An item is set in the collection.
    /// </summary>
    Set,
    /// <summary>
    /// An item is removed from the collection.
    /// </summary>
    RemoveAt,
    /// <summary>
    /// Collection is cleared.
    /// </summary>
    Clear,
    /// <summary>
    /// All operations for the tick have been processed. This only occurs on clients as the server is unable to be aware of when the user is done modifying the list.
    /// </summary>
    Complete
}
[Serializable]
public class ObservableList<T> : IList<T>, IReadOnlyList<T>
{
    [Serializable]
    private struct CachedonChange
    {
        internal readonly ObservableListOperation Operation;
        internal readonly int Index;
        internal readonly T Previous;
        internal readonly T Next;

        public CachedonChange(ObservableListOperation operation, int index, T previous, T next)
        {
            Operation = operation;
            Index = index;
            Previous = previous;
            Next = next;
        }
    }
    [SerializeField]
    private List<T> items;
    [SerializeField]
    private List<CachedonChange> cachedChanges;
    [SerializeField]
    private event Action<ObservableListOperation, int, T, T> onChange;

    public ObservableList()
    {
        items = new List<T>();
        cachedChanges = new List<CachedonChange>();
    }

    public event Action<ObservableListOperation, int, T, T> OnChange
    {
        add { onChange += value; }
        remove { onChange -= value; }
    }

    public T this[int index]
    {
        get { return items[index]; }
        set
        {
            T previous = items[index];
            items[index] = value;
            cachedChanges.Add(new CachedonChange(ObservableListOperation.Set, index, previous, value));
            NotifyListChanged();
        }
    }

    public int Count => items.Count;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        items.Add(item);
        int index = items.Count - 1;
        cachedChanges.Add(new CachedonChange(ObservableListOperation.Add, index, default(T), item));
        NotifyListChanged();
    }

    public void Insert(int index, T item)
    {
        items.Insert(index, item);
        cachedChanges.Add(new CachedonChange(ObservableListOperation.Insert, index, default(T), item));
        NotifyListChanged();
    }

    public bool Remove(T item)
    {
        int index = items.IndexOf(item);
        if (index >= 0)
        {
            items.RemoveAt(index);
            cachedChanges.Add(new CachedonChange(ObservableListOperation.RemoveAt, index, item, default(T)));
            NotifyListChanged();
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        T item = items[index];
        items.RemoveAt(index);
        cachedChanges.Add(new CachedonChange(ObservableListOperation.RemoveAt, index, item, default(T)));
        NotifyListChanged();
    }

    public void Clear()
    {
        items.Clear();
        cachedChanges.Add(new CachedonChange(ObservableListOperation.Clear, -1, default(T), default(T)));
        NotifyListChanged();
    }

    public bool Contains(T item)
    {
        return items.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return items.GetEnumerator();
    }


    private void NotifyListChanged()
    {
        Debug.Log("NotifyListChanged, cachedChanges.Count: " + cachedChanges.Count);
        foreach (var change in cachedChanges)
        {
            onChange?.Invoke(change.Operation, change.Index, change.Previous, change.Next);
        }
        cachedChanges.Clear();
    }

    public int IndexOf(T item)
    {
        throw new System.NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new System.NotImplementedException();
    }
}