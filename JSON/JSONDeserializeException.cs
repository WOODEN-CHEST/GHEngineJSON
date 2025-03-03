using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONDeserializeException : JSONException
{
    public JSONDeserializeException(string message, int line, int column)
        : base($"Failed to deserialize JSON on line {line}, column {column}. {message}") { }
}
