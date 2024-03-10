using System;
using System.Collections.Generic;
using System.Linq;

public class DictionaryObservable<TKey, TValue> : Dictionary<TKey, TValue>
{
    // Event to notify when the dictionary changes
    public event Action OnDictionaryChanged;

    public new void Add(TKey key, TValue value)
    {
        base.Add(key, value);
        NotifyDictionaryChanged();
    }

    public new bool Remove(TKey key)
    {
        bool removed = base.Remove(key);
        if (removed)
            NotifyDictionaryChanged();
        return removed;
    }

    // Add other dictionary modification methods as needed

    private void NotifyDictionaryChanged()
    {
        OnDictionaryChanged?.Invoke();
    }
}