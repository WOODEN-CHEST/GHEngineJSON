using GHEngine.IO.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngineJSON.JSON;

public class JSONSchemaException : JSONEntryException
{
    public JSONSchemaException(string message) : base(message) { }
}