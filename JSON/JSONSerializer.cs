using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONSerializer
{
    // Methods.
    public string Serialize(object? JSONObject, bool isFormatted)
    {
        return SerializeObject(JSONObject, isFormatted, 1);
    }


    // Private methods.
    private string SerializeObject(object? JSONObject, bool isFormatted, int indentLevel)
    {
        if (JSONObject == null)
        {
            return "null";
        }
        else if (JSONObject is string StringObject)
        {
            return $"\"{FormatString(StringObject)}\"";
        }
        else if (JSONObject is byte or sbyte or short or ushort or int or uint or long or ulong)
        {
            return JSONObject.ToString()!;
        }
        else if (JSONObject is float FloatObject)
        {
            return FloatObject.ToString(CultureInfo.InvariantCulture);
        }
        else if (JSONObject is double DoubleObject)
        {
            return DoubleObject.ToString(CultureInfo.InvariantCulture);
        }
        else if (JSONObject is bool BooleanObject)
        {
            return BooleanObject.ToString().ToLower();
        }
        else if (JSONObject is JSONCompound JSONCompound)
        {
            return SerializeCompound(JSONCompound, isFormatted, indentLevel);
        }
        else if (JSONObject is JSONList JSONArray)
        {
            return SerializeList(JSONArray, isFormatted, indentLevel);
        }
        else
        {
            throw new ArgumentException($"Invalid JSON object type: {JSONObject.GetType()}");
        }
    }

    private string SerializeList(JSONList list, bool isFormatted, int indentLevel)
    {
        if (list.Count == 0)
        {
            return $"{JSONSyntax.ARRAY_OPEN}{JSONSyntax.ARRAY_CLOSE}";
        }

        StringBuilder Data = new();
        Data.Append(JSONSyntax.ARRAY_OPEN);
        for (int i = 0; i < list.Count; i++)
        {
            if (i != 0)
            {
                Data.Append(JSONSyntax.SEPARATOR);
            }

            AppendIfFormatted(Data, '\n', isFormatted);
            AppendIndent(Data, indentLevel, isFormatted);

            Data.Append(SerializeObject(list[i], isFormatted, indentLevel + 1));
        }

        AppendIfFormatted(Data, '\n', isFormatted);
        AppendIndent(Data, indentLevel - 1, isFormatted);

        Data.Append(JSONSyntax.ARRAY_CLOSE);
        return Data.ToString();
    }

    private string SerializeCompound(JSONCompound compound, bool isFormatted, int indentLevel)
    {
        if (compound.EntryCount == 0)
        {
            return $"{JSONSyntax.COMPOUND_OPEN}{JSONSyntax.COMPOUND_CLOSE}";
        }

        StringBuilder Data = new();
        Data.Append(JSONSyntax.COMPOUND_OPEN);
        int EntryIndex = 0;
        foreach (KeyValuePair<string, object?> Entry in compound)
        {
            if (EntryIndex != 0)
            {
                Data.Append(JSONSyntax.SEPARATOR);
            }

            AppendIfFormatted(Data, '\n', isFormatted);
            AppendIndent(Data, indentLevel, isFormatted);

            Data.Append($"{JSONSyntax.QUOTE}{FormatString(Entry.Key)}{JSONSyntax.QUOTE}");
            Data.Append(JSONSyntax.VALUE_DEFINITION);
            AppendIfFormatted(Data, ' ', isFormatted);
            Data.Append(SerializeObject(Entry.Value, isFormatted, indentLevel + 1));
            EntryIndex++;
        }

        AppendIfFormatted(Data, '\n', isFormatted);
        AppendIndent(Data, indentLevel - 1, isFormatted);
        Data.Append(JSONSyntax.COMPOUND_CLOSE);

        return Data.ToString();
    }

    private void AppendIfFormatted(StringBuilder builder, char character, bool isFormatted)
    {
        if (isFormatted)
        {
            builder.Append(character);
        }
    }

    private void AppendIndent(StringBuilder builder, int level, bool ifFormatted)
    {
        if (!ifFormatted)
        {
            return;
        }

        for (int i = 0; i < level; i++)
        {
            builder.Append("    ");
        }
    }

    private string FormatString(string stringToFormat)
    {
        StringBuilder FormattedData = new(stringToFormat.Length);

        for (int i = 0; i < stringToFormat.Length; i++)
        {
            char Character = stringToFormat[i];
            switch (Character)
            {
                case '\\':
                    FormattedData.Append("\\\\");
                    break;

                case '"':
                    FormattedData.Append("\\\"");
                    break;

                case '\n':
                    FormattedData.Append("\\n");
                    break;

                case '\r':
                    FormattedData.Append("\\r");
                    break;

                case '\t':
                    FormattedData.Append("\\t");
                    break;

                case '\f':
                    FormattedData.Append("\\f");
                    break;

                case '\b':
                    FormattedData.Append("\\b");
                    break;

                default:
                    FormattedData.Append(Character);
                    break;
            }
        }

        return FormattedData.ToString();
    }
}