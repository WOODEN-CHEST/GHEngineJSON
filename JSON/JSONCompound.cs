

using GHEngineJSON.JSON;
using System.Collections;
using System.ComponentModel.Design;
using System.Data.SqlTypes;

namespace GHEngine.IO.JSON;

public class JSONCompound : IEnumerable<KeyValuePair<string, object?>>
{
    // Fields.
    public int EntryCount => _entries.Count;


    // Private fields.
    private readonly Dictionary<string, object?> _entries = new();


    // Constructors.
    public JSONCompound() { }


    // Methods.
    public void Add(string key, object? value)
    {
        if (value != null && !(value is byte or sbyte or short or ushort or int or uint or long or ulong
            or float or double or bool or string or JSONCompound or JSONList))
        {
            throw new JSONEntryException($"Invalid type of JSON entry: {value.GetType().FullName}");
        }
        _entries.Add(key,value);
    }

    public bool Remove(string key)
    {
        return _entries.Remove(key);
    }

    public void Clear()
    {
        _entries.Clear();
    }

    public bool Get<T>(string key, out T? value)
    {
        return Get(key, false, true, default, out value);
    }

    public T GetOrDefault<T>(string key, T defaultValue)
    {
        Get(key, false, true, defaultValue, out T? Value);
        return Value!;
    }

    public T GetVerified<T>(string key)
    {
        Get(key, true, false, default, out T? Value);
        return Value!;
    }

    public bool GetOptionalVerified<T>(string key, out T? value)
    {
        return Get(key, true, true, default, out value);
    }

    public T GetVerifiedOrDefault<T>(string key, T defaultValue)
    {
        Get(key, true, true, defaultValue, out T? Value);
        return Value!;
    }


    // Private methods.
    private bool Get<T>(string key, bool isVerified, bool isOptional, T? defaultValue, out T? value)
    {
        value = defaultValue;
        if (!_entries.TryGetValue(key, out object? Value))
        {
            if (isVerified && !isOptional)
            {
                throw new JSONSchemaException($"Compound does not contain the entry \"{key}\"");
            }
            return false;
        }

        if (isVerified && (Value == null))
        {
            throw new JSONSchemaException($"Compound entry \"{key}\" is null, expected type {typeof(T).FullName}");
        }

        try
        {
            value = (T?)Value;
            return true;
        }
        catch (InvalidCastException)
        {
            if (isVerified)
            {
                throw new JSONSchemaException($"Compound entry \"{key}\" is of type " +
                    $"{Value?.GetType().FullName}, expected {typeof(T).FullName}");
            }
            return false;
        }
    }



    // Inherited methods.
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        foreach (KeyValuePair<string, object?> Pair in _entries)
        {
            yield return Pair;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    // Operators.
    public object? this[string key]
    {
        get => _entries[key];
        set => _entries[key] = value;
    }
}