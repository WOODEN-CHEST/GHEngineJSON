using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public static class JSONUtilities
{
    public static double GetDouble(object value)
    {
        if (value is double DoubleValue)
        {
            return DoubleValue;
        }
        if (value is long LongValue)
        {
            return (double)LongValue;
        }
        throw new JSONSchemaException("Expected decimal value");
    }
}