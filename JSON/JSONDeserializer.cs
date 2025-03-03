using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.JSON;

public class JSONDeserializer
{
    // Constructors.
    public JSONDeserializer() { }


    // Methods.
    public object? Deserialize(string data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        DeserializationState State = new(data);
        SkipUnitlNonWhiteSpace(State);
        return ParseValue(State);
    }


    // Private methods.
    private void SkipUnitlNonWhiteSpace(DeserializationState state)
    {
        while (char.IsWhiteSpace(state.GetChar()) && state.IsIndexInBounds)
        {
            state.IncrementIndex();
        }
    }

    private object? ParseValue(DeserializationState state)
    {
        char Character = state.GetChar();
        if (Character == JSONSyntax.COMPOUND_OPEN)
        {
            return ParseCompound(state);
        }
        else if (Character == JSONSyntax.ARRAY_OPEN)
        {
            return ParseList(state);
        }
        else if (char.IsAsciiLetter(Character))
        {
            return ParseLiteralValue(state);
        }
        else if (char.IsDigit(Character) || (Character == JSONSyntax.NUMBER_SIGN_PLUS)
            || (Character == JSONSyntax.NUMBER_SIGN_MINUS))
        {
            return ParseNumber(state);
        }
        else if (Character == JSONSyntax.QUOTE)
        {
            return ParseString(state);
        }
        else
        {
            throw new JSONDeserializeException("Invalid value", state.Line, state.Column);
        }
    }

    private JSONCompound ParseCompound(DeserializationState state)
    {
        if (state.GetChar() != JSONSyntax.COMPOUND_OPEN)
        {
            throw new JSONDeserializeException($"Expected '{JSONSyntax.COMPOUND_OPEN}'.", state.Line, state.Column);
        }
        state.IncrementIndex();

        JSONCompound Compound = new();

        SkipUnitlNonWhiteSpace(state);
        bool ExpectingValue = false;
        while (((state.GetChar() != JSONSyntax.COMPOUND_CLOSE) || ExpectingValue) && state.IsIndexInBounds)
        {
            string Key = ParseString(state);
            SkipUnitlNonWhiteSpace(state);
            if (state.GetChar() != JSONSyntax.VALUE_DEFINITION)
            {
                throw new JSONDeserializeException("Expected value assignment after key.", state.Line, state.Column);
            }
            state.IncrementIndex();
            SkipUnitlNonWhiteSpace(state);

            object? Value = ParseValue(state);
            Compound.Add(Key, Value);

            SkipUnitlNonWhiteSpace(state);
            ExpectingValue = state.GetChar() == JSONSyntax.SEPARATOR;
            if (ExpectingValue)
            {
                state.IncrementIndex();
            }
            SkipUnitlNonWhiteSpace(state);
        }

        if (state.GetChar() != JSONSyntax.COMPOUND_CLOSE)
        {
            throw new JSONDeserializeException($"Expected '{JSONSyntax.COMPOUND_CLOSE}'.", state.Line, state.Column);
        }
        state.IncrementIndex();
        return Compound;
    }

    private JSONList ParseList(DeserializationState state)
    {
        if (state.GetChar() != JSONSyntax.ARRAY_OPEN)
        {
            throw new JSONDeserializeException($"Expected '{JSONSyntax.ARRAY_OPEN}'.", state.Line, state.Column);
        }
        state.IncrementIndex();

        JSONList Array = new();

        SkipUnitlNonWhiteSpace(state);
        bool ExpectingValue = false;
        while (((state.GetChar() != JSONSyntax.ARRAY_CLOSE) || ExpectingValue) && state.IsIndexInBounds)
        {
            Array.Add(ParseValue(state));

            SkipUnitlNonWhiteSpace(state);
            ExpectingValue = state.GetChar() == JSONSyntax.SEPARATOR;
            if (ExpectingValue)
            {
                state.IncrementIndex();
            }
            SkipUnitlNonWhiteSpace(state);
        }

        if (state.GetChar() != JSONSyntax.ARRAY_CLOSE)
        {
            throw new JSONDeserializeException($"Expected '{JSONSyntax.ARRAY_CLOSE}'.", state.Line, state.Column);
        }
        state.IncrementIndex();
        return Array;
    }

    private string ParseString(DeserializationState state)
    {
        if (state.GetChar() != '"')
        {
            throw new JSONDeserializeException("Missing starting quote for string.", state.Line, state.Column);
        }
        state.IncrementIndex();

        StringBuilder Builder = new();
        while ((state.GetChar() != JSONSyntax.QUOTE) && state.IsIndexInBounds)
        {
            if (state.GetChar() != JSONSyntax.ESCAPE_CHARACTER)
            {
                Builder.Append(state.GetChar());
                state.IncrementIndex();
                continue;
            }

            state.IncrementIndex();
            if (!state.IsIndexInBounds)
            {
                throw new JSONDeserializeException("Incomplete escape sequence", state.Line, state.Column);
            }

            Builder.Append(EscapedCharToChar(state, state.GetChar()));
            state.IncrementIndex();
        }

        if (state.GetChar() != '"')
        {
            throw new JSONDeserializeException("Missing ending quote for string.", state.Line, state.Column);
        }
        state.IncrementIndex();
        return Builder.ToString();
    }

    private char EscapedCharToChar(DeserializationState state, char escapedChar)
    {
        switch (escapedChar)
        {
            case 't':
                return '\t';
            case 'n':
                return '\n';
            case 'f':
                return '\f';
            case 'r':
                return '\r';
            case 'b':
                return '\b';
            case JSONSyntax.CODEPOINT_INDICATOR:
                return ReadCodepoint(state);
            default:
                return escapedChar;
        }
    }

    private char ReadCodepoint(DeserializationState state)
    {
        if (state.GetChar() != 'u')
        {
            throw new JSONDeserializeException($"Missing indicator '{JSONSyntax.CODEPOINT_INDICATOR}' for codepoint.",
                state.Line, state.Column);
        }

        Span<char> Codepoint = stackalloc char[4];
        for (int i = 0; i < 4; i++)
        {
            state.IncrementIndex();
            if (!state.IsIndexInBounds)
            {
                throw new JSONDeserializeException("Incomplete codepoint!", state.Line, state.Column);
            }
            Codepoint[i] = state.GetChar();
        }
        try
        {
            return (char)Convert.ToInt32(new string(Codepoint), 16);
        }
        catch (FormatException)
        {
            throw new JSONDeserializeException($"Invalid codepoint {new string(Codepoint)}.", state.Line, state.Column);
        }
    }

    private object? ParseLiteralValue(DeserializationState state)
    {
        StringBuilder Builder = new();
        while (char.IsAsciiLetter(state.GetChar()))
        {
            Builder.Append(state.GetChar());
            state.IncrementIndex();
        }
        string Word = Builder.ToString();

        switch (Word)
        {
            case "null":
                return null;

            case "true":
                return true;

            case "false":
                return false;

            default:
                throw new JSONDeserializeException($"Unknown literal \"{Word}\".", state.Line, state.Column);
        }
    }

    private object ParseNumber(DeserializationState state)
    {
        StringBuilder Builder = new();
        while (char.IsDigit(state.GetChar()) || (state.GetChar() == JSONSyntax.NUMBER_SEPARATOR)
            || (char.ToLower(state.GetChar()) == JSONSyntax.NUMBER_EXPONENT) || (state.GetChar() == JSONSyntax.NUMBER_SIGN_PLUS)
            || (state.GetChar() == JSONSyntax.NUMBER_SIGN_MINUS))
        {
            Builder.Append(state.GetChar());
            state.IncrementIndex();
        }
        string Number = Builder.ToString();

        if (long.TryParse(Number, CultureInfo.InvariantCulture, out long LongValue))
        {
            return LongValue;
        }
        else if (double.TryParse(Number, CultureInfo.InvariantCulture, out double DoubleValue))
        {
            return DoubleValue;
        }
        else
        {
            throw new JSONDeserializeException($"Couldn't parse number \"{Number}\"", state.Line, state.Column);
        }
    }


    // Types.
    private class DeserializationState
    {
        // Fields.
        public string Data { get; }
        public int Index { get; private set; } = 0;
        public int Line { get; private set; } = 1;
        public int Column { get; private set; } = 1;
        public bool IsIndexInBounds => Index < Data.Length;


        // Constructors.
        public DeserializationState(string data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }


        // Methods.
        public char GetChar()
        {
            return Index >= Data.Length ? '\0' : Data[Index];
        }

        public void IncrementIndex()
        {
            if (GetChar() == '\n')
            {
                Line++;
                Column = 1;
            }
            else
            {
                Column++;
            }
            Index++;
        }
    }
}