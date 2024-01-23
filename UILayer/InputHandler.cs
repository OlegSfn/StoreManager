namespace UILayer;

public static class InputHandler
{
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
    
    public static void WaitForUserInput(string msg)
    {
        Printer.PrintWarning(msg, false);
        Console.ReadKey(true);
        Console.WriteLine();
    }
    
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