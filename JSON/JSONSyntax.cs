using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

internal static class JSONSyntax
{
    public const char COMPOUND_OPEN = '{';
    public const char COMPOUND_CLOSE = '}';
    public const char ARRAY_OPEN = '[';
    public const char ARRAY_CLOSE = ']';
    public const char SEPARATOR = ',';
    public const char QUOTE = '"';
    public const char NUMBER_SEPARATOR = '.';
    public const char NUMBER_EXPONENT = 'e';
    public const char NUMBER_SIGN_PLUS = '+';
    public const char NUMBER_SIGN_MINUS = '-';
    public const char VALUE_DEFINITION = ':';
    public const char ESCAPE_CHARACTER = '\\';
    public const char CODEPOINT_INDICATOR = 'u';

    public const string LITERAL_NULL = "null";
    public const string LITERAL_TRUE = "true";
    public const string LITERAL_FALSE = "false";
}