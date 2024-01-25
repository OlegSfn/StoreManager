using DatabaseLayer;
using Entities;
using UILayer;

namespace DataLayer;

/// <summary>
/// Provides functionality for managing application settings.
/// </summary>
public static class SettingsManager
{
    private static readonly string s_settingsFileName = "settings.json";
    
    /// <summary>
    /// Saves the current application settings to a JSON file.
    /// </summary>
    /// <param name="silentSave">If true, no confirmation message will be printed.</param>
    public static void SaveSettings(bool silentSave)
    {
        try
        {
            using StreamWriter sr = new StreamWriter(s_settingsFileName);
            Console.SetOut(sr);
            JsonParser.WriteJson(Storage.S_CurSettings);
            if (!silentSave)
                Printer.PrintInfo("Настройки сохранены.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Console.SetOut(Storage.S_StandardOutput);
        }
    }
    
    /// <summary>
    /// Loads application settings from a JSON file.
    /// </summary>
    public static void LoadSettings()
    {

        if (!File.Exists(s_settingsFileName))
        {
            LoadDefaultSettings();
            return;
        }
        
        try
        {
            using StreamReader sr = new StreamReader(s_settingsFileName);
            Console.SetIn(sr);
            Storage.S_CurSettings = JsonParser.ReadJson<SettingsData>()[0];
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            LoadDefaultSettings();
        }
        finally
        {
            Console.SetIn(Storage.S_StandardInput);
        }
    }
    
    /// <summary>
    /// Loads default application settings and saves them to a JSON file.
    /// </summary>
    public static void LoadDefaultSettings()
    {
        Storage.S_CurSettings = new SettingsData();
        SaveSettings(true);
    }

    /// <summary>
    /// Opens a menu for modifying application settings.
    /// </summary>
    public static void OpenSettings()
    {
        int lastSelectedMenuPoint = 0;
        while (true)
        {
            MenuPoint enterData = new MenuPoint($"Откуда считывать данные? ({Printer.EnterDataOptionToString(Storage.S_CurSettings.EnterDataChoice)})", ChangeDataGettingOption);
            MenuPoint whereToShowResult = new MenuPoint($"Где показывать результат? ({Printer.WhereToShowResultOptionToString(Storage.S_CurSettings.ShowResultChoice)})", ChangeWhereToShowResultOption);
            MenuPoint howToShowResult = new MenuPoint($"Как показывать результат? ({Printer.HowToShowResultOptionToString(Storage.S_CurSettings.ViewingMode)})", ChangeHowToShowResultOption);
            MenuPoint instantFileOpening = new MenuPoint($"Открывать автоматически файл, в который были сохранены данные? ({Printer.BoolToYesOrNo(Storage.S_CurSettings.NeedOpenFileAfterWriting)})", ChangeInstantOpeningOption);
            MenuPoint favouriteFileInput = new MenuPoint($"Любимый путь до файла ввода - {(Storage.S_CurSettings.FavouriteInputFile != string.Empty ? Storage.S_CurSettings.FavouriteInputFile : "не выбран")}", ChangeFavouriteInputFile);
            MenuPoint favouriteFileOutput = new MenuPoint($"Любимый путь до файла вывода - {(Storage.S_CurSettings.FavouriteOutputFile != string.Empty ? Storage.S_CurSettings.FavouriteOutputFile : "не выбран")}", ChangeFavouriteOutputFile);
            MenuPoint returnToMainMenu = new MenuPoint("Вернуться в меню.");
            Menu settingsMenu = new Menu(new[] { enterData, whereToShowResult, howToShowResult, instantFileOpening, favouriteFileInput, favouriteFileOutput, returnToMainMenu});
            settingsMenu.SelectedMenuPoint = lastSelectedMenuPoint;
            settingsMenu.HandleUsing();
            SaveSettings(true);
            lastSelectedMenuPoint = settingsMenu.SelectedMenuPoint;
            if (settingsMenu.SelectedMenuPoint == 6)
                return;
        }
    }

    /// <summary>
    /// Changes the option for how to get data.
    /// </summary>
    private static void ChangeDataGettingOption()
    {
        int length = Enum.GetValues(typeof(ConsoleFileOption)).Cast<int>().Max();
        int curValue = (int)Storage.S_CurSettings.EnterDataChoice;
        Storage.S_CurSettings.EnterDataChoice = (ConsoleFileOption)(curValue%length+1);
    }
    
    /// <summary>
    /// Changes the option for where to show the result.
    /// </summary>
    private static void ChangeWhereToShowResultOption()
    {
        int length = Enum.GetValues(typeof(ConsoleFileOption)).Cast<int>().Max();
        int curValue = (int)Storage.S_CurSettings.ShowResultChoice;
        Storage.S_CurSettings.ShowResultChoice = (ConsoleFileOption)(curValue%length+1);
    }
    
    /// <summary>
    /// Changes the option for how to show the result.
    /// </summary>
    private static void ChangeHowToShowResultOption()
    {
        int length = Enum.GetValues(typeof(ViewingMode)).Cast<int>().Max();
        int curValue = (int)Storage.S_CurSettings.ViewingMode;
        Storage.S_CurSettings.ViewingMode = (ViewingMode)(curValue%length+1);
    }
    
    /// <summary>
    /// Toggles the option for instant file opening after writing data.
    /// </summary>
    private static void ChangeInstantOpeningOption()
        => Storage.S_CurSettings.NeedOpenFileAfterWriting = !Storage.S_CurSettings.NeedOpenFileAfterWriting;
    
    /// <summary>
    /// Changes the favorite input file path in the current settings based on user input.
    /// </summary>
    private static void ChangeFavouriteInputFile()
    {
        string? filePath = InputHandler.GetFilePathToJson($"Введите путь до json файла или \"{Storage.S_ExitString}\", чтобы сбросить: ");
        Storage.S_CurSettings.FavouriteInputFile = filePath ?? string.Empty;
    }

    /// <summary>
    /// Changes the favorite output file path in the current settings based on user input.
    /// </summary>
    private static void ChangeFavouriteOutputFile()
    {
        string? filePath = InputHandler.GetValidPath($"Введите путь до файла, в который нужно сохранять результат или \"{Storage.S_ExitString}\", чтобы сбросить: ");
        Storage.S_CurSettings.FavouriteOutputFile = filePath ?? string.Empty;
    }
}