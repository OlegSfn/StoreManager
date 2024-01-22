using System.Text;
using Entities;

namespace DataLayer;

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

public enum ValueType
{
    None,
    String,
    Int
}

public static class JsonParser
{
    
    public static void WriteJson(DataBlock dataBlock)
    {
        foreach (string line in dataBlock.FormJson().Split(Environment.NewLine))
            Console.WriteLine(line);
    }

    public static List<DataType> ReadJson<T>() where T : DataType, new()
    {
        int lineNumber = 1, characterNumber = 1;
        string input = FormInput();
        Stack<ParseState> parseStates = new Stack<ParseState>();
        ParseState curState = ParseState.None;
        List<DataType> dataTypes = new List<DataType>();
        T curStore = new T();
        string curKey = "", curVal = "";
        ValueType curValueType = ValueType.None;
        void ThrowExceptionWithComments(string exceptionMessage, char character)
        {
            throw new Exception($"{exceptionMessage}, got {character} in line {lineNumber} on character {characterNumber}");
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
                        curValueType = ValueType.Int;
                    }
                    else if (!char.IsWhiteSpace(c))
                        ThrowExceptionWithComments($"Expected an array item or end of the array", c);
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
                        dataTypes.Add((T)curStore.Clone());
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
                    if (c == '"')
                    {
                        curValueType = ValueType.String;
                        parseStates.Push(curState);
                        curState = ParseState.ReadingValue;
                    }
                    else if (c == '[')
                    {
                        parseStates.Push(curState);
                        curState = ParseState.InArray;
                    }
                    else if (char.IsDigit(c))
                    {
                        curValueType = ValueType.Int;
                        parseStates.Push(curState);
                        curState = ParseState.ReadingValue;
                        curVal += c;
                    }
                    else if (!char.IsWhiteSpace(c) && c != ':')
                        ThrowExceptionWithComments($"Expected value for {curKey}", c);
                    break;
                case ParseState.ReadingValue:
                    if (curValueType == ValueType.String && c == '"')
                    {
                        if (parseStates.Peek() == ParseState.InArray)
                            curVal += StoreData.S_secretSep;
                        else
                        {
                            curStore[curKey] = curVal;
                            (curKey, curVal) = ("", "");
                        }               
                        parseStates.Push(curState);
                        curState = ParseState.ExpectingEnd;
                    }
                    else if (curValueType == ValueType.Int && !char.IsDigit(c)) //TODO: Make double
                    {
                        if (parseStates.Peek() == ParseState.InArray)
                            curVal += StoreData.S_secretSep;
                        else
                        {
                            curStore[curKey] = curVal;
                            (curKey, curVal) = ("", "");
                        } 
                        
                        if (c == ',')
                        {
                            if (parseStates.Peek() == ParseState.InArray)
                            {
                                curState = parseStates.Pop();
                                break;
                            }

                            parseStates.Pop();
                            parseStates.Pop();
                            curState = parseStates.Pop();
                        }
                        else
                        {
                            parseStates.Push(curState);
                            curState = ParseState.ExpectingEnd;
                        }
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

                        parseStates.Pop();
                        parseStates.Pop();
                        curState = parseStates.Pop();
                    }
                    else if (c == ']')
                    {
                        if (parseStates.Peek() == ParseState.InArray)
                        {
                            parseStates.Pop();
                            curState = ParseState.None;
                            break;
                        }
                        
                        curStore[curKey] = curVal;
                        (curKey, curVal) = ("", "");
                        parseStates.Pop();
                        parseStates.Pop();
                        parseStates.Pop();
                    }
                    else if (c == '}')
                    {
                        if (parseStates.Peek() == ParseState.ReadingValue)
                        {
                            
                        }
                        else if (parseStates.Peek() == ParseState.ReadingKey)
                        {
                            parseStates.Pop();
                            parseStates.Pop();
                        }
                        
                        dataTypes.Add((T)curStore.Clone());
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

    //TODO: Remake on StringBuilder
    private static string FormInput()
    {
        string line;
        StringBuilder sb = new StringBuilder();
        while ((line = Console.ReadLine()) != "exit" && line != null)
        {
            sb.Append(line);
            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }
}