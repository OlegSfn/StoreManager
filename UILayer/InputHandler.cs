namespace UILayer;

/// <summary>
/// Handles user input in a safe manner.
/// </summary>
public static class InputHandler
{
    /// <summary>
    /// Reads a line from the console, handling null input.
    /// </summary>
    /// <param name="msg">The message to display before reading input.</param>
    /// <param name="errorMsg">The error message to display if input is null.</param>
    /// <returns>The user input.</returns>
    public static string SafeReadline(string msg, string errorMsg)
    {
        while (true)
        {
            Console.Write(msg);
            string? userInp = Console.ReadLine();
            if (userInp == null)
                Printer.PrintError(errorMsg);
            else
                return userInp;
        }
    }
    
    /// <summary>
    /// Asks the user a yes/no question.
    /// </summary>
    /// <param name="question">The question to ask.</param>
    /// <param name="yesKey">The key for 'yes' response.</param>
    /// <param name="enterNewLine">Whether to print the question on a new line.</param>
    /// <returns>True if the user answers 'yes', false otherwise.</returns>
    public static bool AskUserYesOrNo(string question, ConsoleKey yesKey, bool enterNewLine=true)
    {
        if (enterNewLine)
            Printer.PrintWarning(question);
        else
            Printer.PrintWarning(question, false);
        
        Console.Write($"Если да, то нажмите {yesKey}, иначе любую кнопку: ");
        if (Console.ReadKey(true).Key == yesKey)
        {
            Console.WriteLine();
            return true;
        }

        Console.WriteLine();
        return false;
    }
    
    /// <summary>
    /// Gets a number from the user within a specified range.
    /// </summary>
    /// <param name="minNum">The minimum allowed number.</param>
    /// <param name="maxNum">The maximum allowed number.</param>
    /// <returns>The user-entered number within the specified range.</returns>
    public static int GetUserNumberInRange(int minNum, int maxNum)
    {
        while (true)
        {
            string msg = $"Введите число в отрезке от {minNum} до {maxNum}: ";
            if (int.TryParse(SafeReadline(msg, "Введена недопустимая строка."), out int res))
            {
                if ((minNum <= res) && (res <= maxNum))
                    return res;

                Printer.PrintError($"Введенное число не попадает в отрезок от {minNum} до {maxNum}");
            }
            else
            {
                Printer.PrintError("Вы ввели не число.");
            }
        }
    }
    
    /// <summary>
    /// Waits for the user to press any key.
    /// </summary>
    /// <param name="msg">The message to display before waiting.</param>
    public static void WaitForUserInput(string msg)
    {
        Printer.PrintWarning(msg, false);
        Console.ReadKey(true);
        Console.WriteLine();
    }
    
    /// <summary>
    /// Gets a file path from the user, ensuring it ends with ".json".
    /// </summary>
    /// <param name="msg">The message to display before getting the file path.</param>
    /// <returns>The valid file path entered by the user.</returns>
    public static string GetFilePathToJson(string msg)
    {
        while (true)
        {
            string filePath = SafeReadline(msg, "Введена недопустимая строка.");
            filePath = filePath.EndsWith(".json") ? filePath : filePath + ".json";
            if (filePath.All(x => !Path.GetInvalidPathChars().Contains(x)))
            {
                if (File.Exists(filePath))
                    return filePath;
                
                Printer.PrintError("Файл по указанному пути не существует.");
            }
            else
                Printer.PrintError("Название файла содержит недопустимые символы.");
        }
    }
    
    /// <summary>
    /// Gets a valid file path from the user, handling existing files and potential overwrite confirmation.
    /// </summary>
    /// <param name="msg">The message to display before getting the file path.</param>
    /// <returns>The valid file path entered by the user.</returns>
    public static string GetValidPath(string msg)
    {
        while (true)
        {
            string filePath = SafeReadline(msg, "Введена недопустимая строка.");
            filePath = filePath.EndsWith(".json") ? filePath : filePath + ".json";
            if (filePath.All(x => !Path.GetInvalidPathChars().Contains(x)))
            {
                if (File.Exists(filePath))
                {
                    if (AskUserYesOrNo("Файл по указанному пути уже существует, Вы хотели бы его перезаписать?",
                            ConsoleKey.Enter))
                        return filePath;
                    msg = "Введите новый путь до файла: ";
                }
                else
                    return filePath;
            }
            else
                Printer.PrintError("Название файла содержит недопустимые символы.");
        }
    }
}