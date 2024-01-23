using System.Text;
using System.Text.RegularExpressions;
using Entities;

namespace DataLayer;

/// <summary>
/// Represents states during JSON parsing.
/// </summary>
public enum ParseState
{
    None,
    InArray,
    InObject,
    ReadingKey,
    ExpectingValue,
    ReadingValue,
    ExpectingEnd
}

/// <summary>
/// Represents types of values during JSON parsing.
/// </summary>
public enum ValueType
{
    None,
    String,
    Value,
    Bool,
    Null
}

/// <summary>
/// Provides functionality for parsing and formatting JSON data.
/// </summary>
public static class JsonParser
{
    public static readonly string SExitString = "--Return--"; 
    
    /// <summary>
    /// Writes the JSON representation of a <see cref="DataBlock"/> to the console.
    /// </summary>
    /// <param name="dataBlock">The DataBlock to write as JSON.</param>
    public static void WriteJson(DataBlock dataBlock)
    {
        foreach (string line in dataBlock.FormJson().Split(Environment.NewLine))
            Console.WriteLine(line);
    }
    
    /// <summary>
    /// Writes the string representation of a <see cref="DataType"/> to the console.
    /// </summary>
    /// <param name="dataType">The DataType to write as a string.</param>
    public static void WriteJson(DataType dataType)
    { 
        Console.WriteLine(dataType);
    }

    /// <summary>
    /// Reads JSON data and converts it to a list of specified data types.
    /// </summary>
    /// <typeparam name="T">The type of DataType to read from the JSON data.</typeparam>
    /// <returns>A list of DataType objects read from the JSON data.</returns>
    public static List<T> ReadJson<T>() where T : DataType, new()
    {
        int lineNumber = 1, characterNumber = 1;
        string input = FormInput();
        Stack<ParseState> parseStates = new Stack<ParseState>();
        ParseState curState = ParseState.None;
        List<T> dataTypes = new List<T>();
        T curStore = new T();
        string curKey = "", curVal = "";
        bool hasDotInside = false;
        ValueType curValueType = ValueType.None;
        char[] tfnAlph = { 't', 'r', 'u', 'e', 'f', 'a', 'l', 's', 'n', 'u' };
        
        void ThrowExceptionWithComments(string exceptionMessage, char character)
        {
            throw new Exception($"{exceptionMessage}, got {character} in line {lineNumber} on character {characterNumber}");
        }

        void ResetValues()
        {
            curStore[curKey] = curVal;
            (curKey, curVal) = ("", "");
            curValueType = ValueType.None;
            hasDotInside = false;
        }

        ParseState PopFor(int count)
        {
            for (int i = 0; i < count-1; i++)
                parseStates.Pop();
            
            return parseStates.Pop();
        }

        void HandleComma(char c)
        {
            if (c == ',')
            {
                if (parseStates.Peek() == ParseState.InArray)
                {
                    curState = parseStates.Pop();
                    return;
                }

                curState = PopFor(3);
            }
            else
            {
                parseStates.Push(curState);
                curState = ParseState.ExpectingEnd;
            }
        }

        void PushValue()
        {
            if (parseStates.Peek() == ParseState.InArray)
                curVal += DataType.S_SecretSep;
            else
                ResetValues();
        }

        foreach (var c in input)
        {
            switch (curState)
            {
                case ParseState.None:
                    if (c == '[')
                    {
                        parseStates.Push(curState);
                        curState = ParseState.InArray;
                    }
                    else if (c == '{')
                    {
                        parseStates.Push(curState);
                        curState = ParseState.InObject;
                    }
                    else if (!char.IsWhiteSpace(c))
                    {
                        if (dataTypes.Count == 0)
                            throw new Exception("Expected array of objects or object");
                        
                        ThrowExceptionWithComments($"Expected end of data", c);
                    }
                    break;
                case ParseState.InArray when parseStates.Peek() == ParseState.ExpectingValue:
                    if (c == '"')
                    {
                        parseStates.Push(curState);
                        curState = ParseState.ReadingValue;
                        curValueType = ValueType.String;
                    }
                    else if (char.IsDigit(c))
                    {
                        curVal += c;
                        parseStates.Push(curState);
                        curState = ParseState.ReadingValue;
                        curValueType = ValueType.Value;
                    }
                    else if (!char.IsWhiteSpace(c))
                        ThrowExceptionWithComments("Expected an array item or end of the array", c);
                    break;
                case ParseState.InArray when parseStates.Peek() == ParseState.None:
                    if (c == '{')
                    {
                        parseStates.Push(curState);
                        curState = ParseState.InObject;
                    }
                    else if (c == ']')
                        curState = parseStates.Pop();
                    else if (!char.IsWhiteSpace(c))
                        ThrowExceptionWithComments($"Expected new object", c);
                    break;
                case ParseState.InObject:
                    if (c == '"')
                    {
                        parseStates.Push(curState);
                        curState = ParseState.ReadingKey;
                    }
                    else if (c == '}')
                    {
                        curState = parseStates.Pop();
                        dataTypes.Add(curStore);
                        curStore = new T();
                    }
                    else if (!char.IsWhiteSpace(c))
                        ThrowExceptionWithComments($"Expected new field or end of object", c);
                    break;
                case ParseState.ReadingKey:
                    if (c == '"')
                    {
                        parseStates.Push(curState);
                        curState = ParseState.ExpectingValue;
                    }
                    else
                        curKey += c;
                    break;
                case ParseState.ExpectingValue:
                    if (char.IsDigit(c))
                    {
                        curValueType = ValueType.Value;
                        parseStates.Push(curState);
                        curState = ParseState.ReadingValue;
                        curVal += c;
                    }
                    else switch (c)
                    {
                        case '"':
                            curValueType = ValueType.String;
                            parseStates.Push(curState);
                            curState = ParseState.ReadingValue;
                            break;
                        case 't':
                        case 'f':
                            curValueType = ValueType.Bool;
                            parseStates.Push(curState);
                            curState = ParseState.ReadingValue;
                            curVal += c;
                            break;
                        case 'n':
                            curValueType = ValueType.Null;
                            parseStates.Push(curState);
                            curState = ParseState.ReadingValue;
                            curVal += c;
                            break;
                        case '[':
                            parseStates.Push(curState);
                            curState = ParseState.InArray;
                            break;
                        default:
                        {
                            if (!char.IsWhiteSpace(c) && c != ':')
                                ThrowExceptionWithComments($"Expected value for {curKey}", c);
                            break;
                        }
                    }
                    break;
                case ParseState.ReadingValue:
                    if (curValueType == ValueType.String && c == '"')
                    {
                        PushValue();
                        parseStates.Push(curState);
                        curState = ParseState.ExpectingEnd;
                    }
                    else if (curValueType == ValueType.Value && !char.IsDigit(c))
                    {
                        if (c == '.')
                        {
                            if (hasDotInside)
                                ThrowExceptionWithComments("Expected fractional part of value", c);
                            else
                                curVal += c;
                            
                            hasDotInside = true;
                            break;
                        }
                        PushValue();
                        HandleComma(c);
                    }
                    else if (curValueType == ValueType.Bool && !tfnAlph.Contains(c))
                    {
                        if (curVal.StartsWith('t') && curVal != "true")
                            throw new Exception($"Expected true in line {lineNumber} on character {characterNumber}, got {curVal + c}");
                        if (curVal.StartsWith('f') && curVal != "false")
                            throw new Exception($"Expected false in line {lineNumber} on character {characterNumber}, got {curVal + c}");
                        
                        PushValue();
                        HandleComma(c);
                    }
                    else if (curValueType == ValueType.Null && !tfnAlph.Contains(c))
                    {
                        if (curVal.StartsWith('n') && curVal != "null")
                            throw new Exception($"Expected null in line {lineNumber} on character {characterNumber}, got {curVal + c}");
                        
                        PushValue();
                        HandleComma(c);
                    }
                    else
                    {
                        curVal += c;
                    }

                    break;
                case ParseState.ExpectingEnd:
                    if (c == ',')
                    {
                        parseStates.Pop();
                        if (parseStates.Count == 0)
                            throw new Exception("Expected only 1 object");
                        
                        if (parseStates.Peek() == ParseState.InArray || parseStates.Peek() == ParseState.InObject)
                        {
                            curState = parseStates.Pop();
                            break;
                        }

                        if (parseStates.Peek() == ParseState.None)
                        {
                            curState = ParseState.InArray;
                            break;
                        }
                        
                        curState = PopFor(3);
                    }
                    else if (c == ']')
                    {
                        if (parseStates.Peek() == ParseState.InArray)
                        {
                            parseStates.Pop();
                            curState = ParseState.None;
                            break;
                        }
                        
                        ResetValues();
                        PopFor(3);
                    }
                    else if (c == '}')
                    {
                        if (parseStates.Peek() == ParseState.ReadingValue)
                            PopFor(4);
                        else if (parseStates.Peek() == ParseState.ReadingKey)
                            PopFor(2);
                        
                        dataTypes.Add(curStore);
                        curStore = new T();
                    }
                    else if (!char.IsWhiteSpace(c))
                        ThrowExceptionWithComments($"Expected comma", c);
                    break;
            }

            // Not Environment.NewLine because we are reading only 1 char per iteration. (win - \r\n, mac - \n, both have \n).
            if (c == '\n')
            {
                lineNumber++;
                characterNumber = 0;
            }

            characterNumber++;
        }

        return dataTypes;
    }
    
    /// <summary>
    /// Reads JSON data using regular expressions and converts it to a list of specified data types.
    /// </summary>
    /// <typeparam name="T">The type of DataType to read from the JSON data.</typeparam>
    /// <returns>A list of DataType objects read from the JSON data using regular expressions.</returns>
    public static List<DataType> ReadJsonRegex<T>() where T : DataType, new() 
    {
        List<DataType> dataTypes = new List<DataType>();
        T curStore = new T();
        string input = FormatInputForRegex(FormInput());
        string storePattern = @"\{""store_id"":(\d+),""store_name"":""(.*?)"",""location"":""(.*?)"",""employees"":\[(.+?)\],""products"":\[(.+?)\]}";
        foreach (Match store in Regex.Matches(input, storePattern))
        {
            curStore["store_id"] = store.Groups[1].Value;
            curStore["store_name"] = store.Groups[2].Value;
            curStore["location"] = store.Groups[3].Value;
            curStore["employees"] = string.Join(DataType.S_SecretSep, store.Groups[4].Value.Split(',').Select(x => x.Trim('"')));
            curStore["products"] = string.Join(DataType.S_SecretSep, store.Groups[5].Value.Split(',').Select(x => x.Trim('"')));
            dataTypes.Add(curStore);
            curStore = new T();
        }
        
        return dataTypes;
    }
    
    /// <summary>
    /// Reads input from the console until the exit string is encountered.
    /// </summary>
    /// <returns>The formatted input string.</returns>
    private static string FormInput()
    {
        string line;
        StringBuilder sb = new StringBuilder();
        while ((line = Console.ReadLine()) != SExitString && line != null)
        {
            sb.Append(line);
            sb.Append(Environment.NewLine);
        }
        
        return sb.ToString();
    }

    /// <summary>
    /// Formats input string for use with regular expressions.
    /// </summary>
    /// <param name="input">The input string to format.</param>
    /// <returns>The formatted input string.</returns>
    private static string FormatInputForRegex(string input)
    {
        StringBuilder sb = new StringBuilder();
        bool isVerbatim = false;
        foreach (char c in input)
        {
            if (char.IsWhiteSpace(c) && !isVerbatim)
                continue;
            
            if (c == '"')
                isVerbatim = !isVerbatim;

            sb.Append(c);
        }

        return sb.ToString();
    }
}