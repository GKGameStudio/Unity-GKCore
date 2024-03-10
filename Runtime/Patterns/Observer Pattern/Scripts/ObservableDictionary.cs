using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObservableDictionaryOperation : byte
{
    /// <summary>
    /// A key and value have been added to the collection.
    /// </summary>
    Add,
    /// <summary>
    /// Collection has been cleared.
    /// </summary>
    Clear,
    /// <summary>
    /// A key was removed from the collection.
    /// </summary>
    Remove,
    /// <summary>
    /// A value has been set for a key in the collection.
    /// </summary>
    Set,
    /// <summary>
    /// All operations for the tick have been processed.
    /// </summary>
    Complete
}

[Serializable]
public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    [Serializable]
    private struct CachedDictionaryChange
    {
        internal readonly ObservableDictionaryOperation Operation;
        internal readonly TKey Key;
        internal readonly TValue PreviousValue;
        internal readonly TValue NextValue;

        public CachedDictionaryChange(ObservableDictionaryOperation operation, TKey key, TValue previousValue, TValue nextValue)
        {
            Operation = operation;
            Key = key;
            PreviousValue = previousValue;
            NextValue = nextValue;
        }
    }

    [SerializeField]
    private Dictionary<TKey, TValue> dictionary;
    [SerializeField]
    private List<CachedDictionaryChange> cachedChanges;
    [SerializeField]
    private event Action<ObservableDictionaryOperation, TKey, TValue, TValue> onChange;

    public ObservableDictionary()
    {
        dictionary = new Dictionary<TKey, TValue>();
        cachedChanges = new List<CachedDictionaryChange>();
    }

    public event Action<ObservableDictionaryOperation, TKey, TValue, TValue> OnChange
    {
        add { onChange += value; }
        remove { onChange -= value; }
    }

    public TValue this[TKey key]
    {
        get { return dictionary[key]; }
        set
        {
            TValue previousValue;
            if (dictionary.TryGetValue(key, out previousValue))
            {
                dictionary[key] = value;
                cachedChanges.Add(new CachedDictionaryChange(ObservableDictionaryOperation.Set, key, previousValue, value));
                NotifyDictionaryChanged();
            }
            else
            {
                Add(key, value);
            }
        }
    }

    public int Count => dictionary.Count;

    public bool IsReadOnly => false;

    public ICollection<TKey> Keys => dictionary.Keys;

    public ICollection<TValue> Values => dictionary.Values;

    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value);
        cachedChanges.Add(new CachedDictionaryChange(ObservableDictionaryOperation.Add, key, default(TValue), value));
        NotifyDictionaryChanged();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((IDictionary<TKey, TValue>)dictionary).Contains(item);
    }

    public bool Remove(TKey key)
    {
        TValue value;
        if (dictionary.TryGetValue(key, out value))
        {
            dictionary.Remove(key);
            cachedChanges.Add(new CachedDictionaryChange(ObservableDictionaryOperation.Remove, key, value, default(TValue)));
            NotifyDictionaryChanged();
            return true;
        }
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return ((IDictionary<TKey, TValue>)dictionary).Remove(item);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return dictionary.TryGetValue(key, out value);
    }

    public void Clear()
    {
        dictionary.Clear();
        cachedChanges.Add(new CachedDictionaryChange(ObservableDictionaryOperation.Clear, default(TKey), default(TValue), default(TValue)));
        NotifyDictionaryChanged();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((IDictionary<TKey, TValue>)dictionary).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void NotifyDictionaryChanged()
    {
        foreach (var change in cachedChanges)
        {
            onChange?.Invoke(change.Operation, change.Key, change.PreviousValue, change.NextValue);
        }
        cachedChanges.Clear();
        onChange?.Invoke(ObservableDictionaryOperation.Complete, default(TKey), default(TValue), default(TValue));
    }
}