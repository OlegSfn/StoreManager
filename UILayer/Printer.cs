using System.Diagnostics;
using Entities;

namespace UILayer;

public class Printer
{
    private static int s_maxLen = 20;
    private static int s_maxColumnsOnScreen = 5;
    
    // Thread vars.
    private static int s_lastConsoleWidth = -1;
    private static bool s_threadAlive = false;
    private static DataType[]? s_colleges = null;
    private static int s_startIndex = 0;
    
    public static void PrintCountedArray<T>(T[]? array)
    {
        if (array == null)
            return;
        
        int counter = 1;
        foreach (T element in array)
        {
            Console.WriteLine($"{counter}. {element}");
            counter++;
        }
    }
    
    private static void PrintTable(DataType[]? array, char sep, int startIndex, int count)
    {
        if (array == null)
            return;

        FullClear();
        int[] emptyColIndexes = Array.Empty<int>();
        
        PrintLine(MakeReadableArray(array[0].GetFieldNames(), s_maxLen, emptyColIndexes), sep, startIndex, count);
        foreach (DataType row in array)
            PrintLine(MakeReadableArray(row.GetFieldValues(), s_maxLen, emptyColIndexes), sep, startIndex, count);
        PrintWarning("Чтобы перемещаться по столбцам используйте стрелки влево/вправо, чтобы изменить кол-во столбцов стрелки вверх/вниз, чтобы закончить нажмите Enter: ");
    }
    
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
    
    public static void ShowTable(DataType[]? result)
    {
        s_startIndex = 0;
        s_lastConsoleWidth = Console.WindowWidth;
        s_colleges = result;
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
    /// Monitors and adjusts the width of the console window dynamically while the table is being inspected.
    /// </summary>
    private static void CheckWidth()
    {
        while (s_threadAlive)
        {
            if (Console.WindowWidth != s_lastConsoleWidth)
            {
                s_maxLen = GetMaxLenOfColByConsoleWidth(s_maxColumnsOnScreen);
                PrintTable(s_colleges, ' ', s_startIndex, s_maxColumnsOnScreen);
                s_lastConsoleWidth = Console.WindowWidth;
            }
            Thread.Sleep(16);
        }

    }
    
    /// <summary>
    /// Calculates the maximum length of a column based on the current console window width and the specified column count.
    /// </summary>
    /// <param name="colCount">The number of columns to consider for calculating the maximum length.</param>
    /// <returns>The maximum length of a column based on the console window width and column count.</returns>
    private static int GetMaxLenOfColByConsoleWidth(int colCount)
    {
        return (Console.WindowWidth-(colCount-1)*4) / colCount;
    }

    /// <summary>
    /// Opens the specified file in the default associated application.
    /// </summary>
    /// <param name="filePath">The path to the file to be opened.</param>
    private static void OpenFileInEditor(string filePath)
    {
        try
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = filePath
            };
            process.Start();
        }
        catch (Exception)
        {
            Console.WriteLine("Произошла ошибка при запуске отдельного приложения.");
        }
    }
    
    
    public static void FullClear()
    {
        Console.Clear();
        Console.Write("\x1b[3J");
    }
    
    public static void PrintInfo(string msg, bool endWithNewLine = true)
    {
        Console.ForegroundColor = ConsoleColor.Green;

        if (endWithNewLine)
            Console.WriteLine(msg);
        else
            Console.Write(msg);

        Console.ResetColor();
    }
    
    public static void PrintWarning(string msg, bool endWithNewLine = true)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;

        if (endWithNewLine)
            Console.WriteLine(msg);
        else
            Console.Write(msg);

        Console.ResetColor();
    }
    
    public static void PrintError(string msg, bool endWithNewLine = true)
    {
        Console.ForegroundColor = ConsoleColor.Red;

        if (endWithNewLine)
            Console.WriteLine(msg);
        else
            Console.Write(msg);

        Console.ResetColor();
    }
}