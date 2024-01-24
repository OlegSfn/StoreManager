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
    /// Prompts the user to enter a valid file path to a JSON file, ensuring it has the correct extension.
    /// </summary>
    /// <param name="msg">The message to display as a prompt for the user to enter the file path.</param>
    /// <returns>
    /// A valid file path to a JSON file entered by the user, or null if the user provides no input.
    /// </returns>
    public static string? GetFilePathToJson(string msg)
    {
        while (true)
        {
            Console.Write(msg);
            string? filePath = Console.ReadLine();
            if (filePath == null)
                return null;
            
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
    /// Prompts the user to enter a valid file path to a JSON file, ensuring it has the correct extension
    /// and handling cases where the specified file already exists.
    /// </summary>
    /// <param name="msg">The message to display as a prompt for the user to enter the file path.</param>
    /// <returns>
    /// A valid file path to a JSON file entered by the user, or null if the user provides no input.
    /// </returns>
    public static string? GetValidPath(string msg)
    {
        while (true)
        {
            Console.Write(msg);
            string? filePath = Console.ReadLine();
            if (filePath == null)
                return null;
            
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