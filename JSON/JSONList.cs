using GHEngineJSON.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONList : IList<object?>
{
    // Fields.
    public object? this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public int Count => _items.Count;

    public bool IsReadOnly => false;



    // Private fields.
    private readonly List<object?> _items = new();


    // Methods.
    public bool Get<T>(int index, out T? value)
    {
        return Get(index, false, true, default, out value);
    }

    public T GetOrDefault<T>(int index, T defaultValue)
    {
        Get(index, false, true, defaultValue, out T? Value);
        return Value!;
    }

    public T GetVerified<T>(int index)
    {
        Get(index, true, false, default, out T? Value);
        return Value!;
    }

    public bool GetOptionalVerified<T>(int index, out T? value)
    {
        return Get(index, true, true, default, out value);
    }

    public T GetVerifiedOrDefault<T>(int index, T defaultValue)
    {
        Get(index, true, true, defaultValue, out T? Value);
        return Value!;
    }



    // Private methods.
    private bool Get<T>(int index, bool isVerified, bool isOptional, T? defaultValue, out T? value)
    {
        value = defaultValue;
        if (index >= Count)
        {
            if (isVerified && !isOptional)
            {
                throw new JSONSchemaException($"List entry index was out of bounds {index} for length of {Count}");
            }
            return false;
        }

        try
        {
            value = (T?)_items[index];

            if (isVerified && (value == null))
            {
                throw new JSONSchemaException($"Compound at index {index} is null, expected type {typeof(T).FullName}");
            }

            return true;
        }
        catch (InvalidCastException)
        {
            if (isVerified)
            {
                throw new JSONSchemaException($"List entry at index {index} is of type " +
                    $"{_items[index]?.GetType().FullName}, expected {typeof(T).FullName}");
            }
            return false;
        }
    }


    // Inherited methods.
    public void Add(object? item)
    {
        _items.Add(item);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool Contains(object? item)
    {
        return _items.Contains(item);
    }

    public void CopyTo(object?[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<object> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public int IndexOf(object? item)
    {
        return _items.IndexOf(item);
    }

    public void Insert(int index, object? item)
    {
        _items.Insert(index, item);
    }

    public bool Remove(object? item)
    {
        return _items.Remove(item);
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}