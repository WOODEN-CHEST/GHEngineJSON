using GHEngine.IO.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONSchemaException : JSONEntryException
{
    public JSONSchemaException(string message) : base(message) { }
}