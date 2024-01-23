using Entities;

namespace UILayer;

/// <summary>
/// Provides methods for displaying information in the console.
/// </summary>
public static class Printer
{
    private static int s_maxLen = 20;
    private static int s_maxColumnsOnScreen = 5;
    
    // Thread vars.
    private static int s_lastConsoleWidth = -1;
    private static bool s_threadAlive = false;
    private static PresentationDataType[]? s_dataTypes = null;
    private static int s_startIndex = 0;
    
    /// <summary>
    /// Displays a table of data in the console.
    /// </summary>
    /// <param name="result">The array of PresentationDataType to display as a table.</param>
    public static void ShowTable(PresentationDataType[]? result)
    {
        s_startIndex = 0;
        s_lastConsoleWidth = Console.WindowWidth;
        s_dataTypes = result;
        s_threadAlive = true;
        Thread widthChangerThread = new Thread(CheckWidth);
        widthChangerThread.Start();

        int deletedColumns = 0;
        while (true)
        {
            s_maxLen = GetMaxLenOfColByConsoleWidth(s_maxColumnsOnScreen);
            PrintTable(result, ' ', s_startIndex, s_maxColumnsOnScreen);
            while (!Console.KeyAvailable)
            {
                
            }
            
            ConsoleKey userInp = Console.ReadKey(true).Key;
            if (userInp == ConsoleKey.LeftArrow)
            {
                if (s_startIndex == 0)
                    continue;
                s_startIndex--;
            }
            else if (userInp == ConsoleKey.RightArrow)
            {
                if (s_startIndex + s_maxColumnsOnScreen >= result[0].GetFieldNames().Length - deletedColumns || 
                    GetMaxLenOfColByConsoleWidth(s_maxColumnsOnScreen+1) < 3)
                    continue;
                s_startIndex++;
            }
            else if (userInp == ConsoleKey.UpArrow)
            {
                if (s_maxColumnsOnScreen == result[0].GetFieldNames().Length - deletedColumns || 
                    GetMaxLenOfColByConsoleWidth(s_maxColumnsOnScreen+1) < 3)
                    continue;

                s_maxColumnsOnScreen++;
                if (s_startIndex + s_maxColumnsOnScreen > result[0].GetFieldNames().Length - deletedColumns)
                {
                    if (s_startIndex == 0)
                        s_maxColumnsOnScreen--;
                    else
                        s_startIndex--;
                }
            }
            else if (userInp == ConsoleKey.DownArrow)
            {
                if (s_maxColumnsOnScreen == 1)
                    continue;
                s_maxColumnsOnScreen--;
            }
            else if (userInp == ConsoleKey.Enter)
            {
                s_threadAlive = false;
                widthChangerThread.Join();
                Console.WriteLine();
                return;
            }
        }
    }
    
    /// <summary>
    /// Clears the console screen.
    /// </summary>
    public static void FullClear()
    {
        Console.Clear();
        Console.Write("\x1b[3J");
    }
    
    /// <summary>
    /// Prints informational message to the console.
    /// </summary>
    /// <param name="msg">The message to print.</param>
    /// <param name="endWithNewLine">Indicates whether to end the message with a new line.</param>
    public static void PrintInfo(string msg, bool endWithNewLine = true)
    {
        Console.ForegroundColor = ConsoleColor.Green;

        if (endWithNewLine)
            Console.WriteLine(msg);
        else
            Console.Write(msg);

        Console.ResetColor();
    }
    
    /// <summary>
    /// Prints a warning message to the console.
    /// </summary>
    /// <param name="msg">The warning message to print.</param>
    /// <param name="endWithNewLine">Indicates whether to end the message with a new line.</param>
    public static void PrintWarning(string msg, bool endWithNewLine = true)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;

        if (endWithNewLine)
            Console.WriteLine(msg);
        else
            Console.Write(msg);

        Console.ResetColor();
    }
    
    /// <summary>
    /// Prints an error message to the console.
    /// </summary>
    /// <param name="msg">The error message to print.</param>
    /// <param name="endWithNewLine">Indicates whether to end the message with a new line.</param>
    public static void PrintError(string msg, bool endWithNewLine = true)
    {
        Console.ForegroundColor = ConsoleColor.Red;

        if (endWithNewLine)
            Console.WriteLine(msg);
        else
            Console.Write(msg);

        Console.ResetColor();
    }
    
    /// <summary>
    /// Converts a boolean value to "Да" or "Нет".
    /// </summary>
    /// <param name="b">The boolean value to convert.</param>
    /// <returns>"Да" if true, "Нет" if false.</returns>
    public static string BoolToYesOrNo(bool b) => b ? "Да" : "Нет";

    /// <summary>
    /// Converts a <see cref="ConsoleFileOption"/> value to a human-readable string.
    /// </summary>
    /// <param name="consoleFileOption">The ConsoleFileOption value to convert.</param>
    /// <returns>A string representation of the ConsoleFileOption.</returns>
    public static string WhereToShowResultOptionToString(ConsoleFileOption consoleFileOption)
    {
        return consoleFileOption switch
        {
            ConsoleFileOption.AlwaysAskUser => "Всегда спрашивать",
            ConsoleFileOption.AlwaysWithFile => "Всегда сохранять в файл",
            ConsoleFileOption.AlwaysWithConsole => "Всегда показывать в консоли",
            _ => throw new ArgumentOutOfRangeException(nameof(consoleFileOption), consoleFileOption, null)
        };
    }
    
    /// <summary>
    /// Converts a <see cref="ViewingMode"/> value to a human-readable string.
    /// </summary>
    /// <param name="viewingMode">The ViewingMode value to convert.</param>
    /// <returns>A string representation of the ViewingMode.</returns>
    public static string HowToShowResultOptionToString(ViewingMode viewingMode)
    {
        return viewingMode switch
        {
            ViewingMode.AskUser => "Всегда спрашивать",
            ViewingMode.Json => "В формате JSON",
            ViewingMode.Table => "В формате таблицы",
            _ => throw new ArgumentOutOfRangeException(nameof(viewingMode), viewingMode, null)
        };
    }
    
    /// <summary>
    /// Converts a <see cref="ConsoleFileOption"/> value to a human-readable string.
    /// </summary>
    /// <param name="consoleFileOption">The ConsoleFileOption value to convert.</param>
    /// <returns>A string representation of the ConsoleFileOption.</returns>
    public static string EnterDataOptionToString(ConsoleFileOption consoleFileOption)
    {
        return consoleFileOption switch
        {
            ConsoleFileOption.AlwaysAskUser => "Всегда спрашивать",
            ConsoleFileOption.AlwaysWithFile => "Всегда считывать из файла",
            ConsoleFileOption.AlwaysWithConsole => "Всегда считывать из консоли",
            _ => throw new ArgumentOutOfRangeException(nameof(consoleFileOption), consoleFileOption, null)
        };
    }
    
    /// <summary>
    /// Prints a table of PresentationDataType objects in the console.
    /// </summary>
    /// <param name="array">An array of PresentationDataType objects to display.</param>
    /// <param name="sep">The character used as a separator in the table.</param>
    /// <param name="startIndex">The starting index of the displayed columns.</param>
    /// <param name="count">The number of columns to display.</param>
    private static void PrintTable(PresentationDataType[]? array, char sep, int startIndex, int count)
    {
        if (array == null)
            return;

        FullClear();
        int[] emptyColIndexes = Array.Empty<int>();
        
        PrintLine(MakeReadableArray(array[0].GetFieldNames(), s_maxLen, emptyColIndexes), sep, startIndex, count);
        foreach (PresentationDataType row in array)
            PrintLine(MakeReadableArray(row.GetFieldValues(), s_maxLen, emptyColIndexes), sep, startIndex, count);
        PrintWarning("Чтобы перемещаться по столбцам используйте стрелки влево/вправо, чтобы изменить кол-во столбцов стрелки вверх/вниз, чтобы закончить нажмите Enter: ");
    }
    
    /// <summary>
    /// Prints a line in the table with proper formatting.
    /// </summary>
    /// <param name="line">An array of strings representing a row in the table.</param>
    /// <param name="sep">The character used as a separator in the table.</param>
    /// <param name="startIndex">The starting index of the displayed columns.</param>
    /// <param name="count">The number of columns to display.</param>
    private static void PrintLine(string?[] line, char sep, int startIndex, int count)
    {
        int endIndex = Math.Min(startIndex + count, line.Length);
        for (int i = startIndex; i < endIndex; i++)
        {
            if (i != endIndex-1)
                Console.Write($"{line[i]}{sep}{sep}{sep}{sep}");
            else
                Console.Write($"{line[i]}");
        }
        Console.WriteLine();
    }
    
    /// <summary>
    /// Creates a readable array of strings with proper formatting for displaying in the table.
    /// </summary>
    /// <param name="columns">An array of strings representing columns in the table.</param>
    /// <param name="maxLen">The maximum length of each column.</param>
    /// <param name="emptyColIndexes">An array of indexes indicating empty columns.</param>
    /// <returns>An array of readable strings for displaying in the table.</returns>
    private static string?[] MakeReadableArray(string?[] columns, int maxLen, int[] emptyColIndexes)
    {
        List<string?> readableArr = new List<string?>();
        if (maxLen < 3)
        {
            for (int i = 0; i < readableArr.Count; i++)
                readableArr[i] = ".";
            return readableArr.ToArray();
        }

        for (int i = 0; i < columns.Length; i++)
        {
            if (emptyColIndexes.Contains(i))
                continue;
            
            if (columns[i] == null)
            {
                readableArr.Add(new string(' ', maxLen));
                continue;
            }
            
            int colLen = columns[i].Length;
            if (colLen > maxLen)
                readableArr.Add(columns[i].Remove(maxLen-3, colLen-maxLen+3) + "...");
            else if (colLen < maxLen)
                readableArr.Add(columns[i].PadRight(maxLen));
            else
                readableArr.Add(columns[i]);
        }

        return readableArr.ToArray();
    }
    
    /// <summary>
    /// Monitors changes in console width to update the table accordingly.
    /// </summary>
    private static void CheckWidth()
    {
        while (s_threadAlive)
        {
            if (Console.WindowWidth != s_lastConsoleWidth)
            {
                s_maxLen = GetMaxLenOfColByConsoleWidth(s_maxColumnsOnScreen);
                PrintTable(s_dataTypes, ' ', s_startIndex, s_maxColumnsOnScreen);
                s_lastConsoleWidth = Console.WindowWidth;
            }
            Thread.Sleep(16);
        }

    }
    
    /// <summary>
    /// Calculates the maximum length of each column based on the current console width.
    /// </summary>
    /// <param name="colCount">The number of columns to consider.</param>
    /// <returns>The maximum length of each column.</returns>
    private static int GetMaxLenOfColByConsoleWidth(int colCount)
    {
        return (Console.WindowWidth-(colCount-1)*4) / colCount;
    }
}