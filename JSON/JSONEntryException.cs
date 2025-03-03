using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONEntryException : JSONException
{
    public JSONEntryException(string message) : base(message) { }
}