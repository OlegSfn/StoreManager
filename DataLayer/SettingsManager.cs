using System.Text.Json;
using DatabaseLayer;
using Entities;
using UILayer;

namespace DataLayer;

public static class SettingsManager
{
    private static string s_settingsFileName = "settings.json";
    private static Menu SSettingsMenu;
    
    public static void SaveSettings(bool silentSave)
    {
        try
        {
            using StreamWriter sr = new StreamWriter(s_settingsFileName);
            Console.SetOut(sr);
            JsonParser.WriteJson(Storage.ScurSettings);
            if (!silentSave)
                Printer.PrintInfo("Настройки сохранены.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Console.SetOut(Storage.SStandardOutput);
        }
    }
    
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
            Storage.ScurSettings = (SettingsData)JsonParser.ReadJson<SettingsData>()[0];
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            LoadDefaultSettings();
        }
        finally
        {
            Console.SetIn(Storage.SStandardInput);
        }
    }
    public static void LoadDefaultSettings()
    {
        Storage.ScurSettings = new SettingsData();
        SaveSettings(true);
    }

    public static void OpenSettings()
    {
        int lastSelectedMenuPoint = 0;
        while (true)
        {
            MenuPoint flushConsole = new MenuPoint($"Очищать консоль после работы с данными? ({Printer.boolToYesOrNo(Storage.ScurSettings.NeedFlushingConsole)})", ChangeFlushingOption);
            MenuPoint enterData = new MenuPoint($"Откуда считывать данные? ({Printer.EnterDataOptionToString(Storage.ScurSettings.EnterDataChoice)})", ChangeDataGettingOption);
            MenuPoint whereToShowResult = new MenuPoint($"Где показывать результат? ({Printer.WhereToShowResultOptionToString(Storage.ScurSettings.ShowResultChoice)})", ChangeWhereToShowResultOption);
            MenuPoint howToShowResult = new MenuPoint($"Как показывать результат? ({Printer.HowToShowResultOptionToString(Storage.ScurSettings.ViewingMode)})", ChangeHowToShowResultOption);
            MenuPoint instantFileOpening = new MenuPoint($"Открывать автоматически файл, в который были сохранены данные? ({Printer.boolToYesOrNo(Storage.ScurSettings.NeedOpenFileAfterWriting)})", ChangeInstantOpeningOption);
            MenuPoint favouriteFileInput = new MenuPoint($"Любимый путь до файла ввода - {(Storage.ScurSettings.FavouriteInputFile != string.Empty ? Storage.ScurSettings.FavouriteInputFile : "не выбран")}", ChangeFavouriteInputFile);
            MenuPoint favouriteFileOutput = new MenuPoint($"Любимый путь до файла вывода - {(Storage.ScurSettings.FavouriteOutputFile != string.Empty ? Storage.ScurSettings.FavouriteOutputFile : "не выбран")}", ChangeFavouriteOutputFile);
            MenuPoint returnToMainMenu = new MenuPoint("Вернуться в меню.");
            Menu settingsMenu = new Menu(new[] { flushConsole, enterData, whereToShowResult, howToShowResult, instantFileOpening, favouriteFileInput, favouriteFileOutput, returnToMainMenu});
            settingsMenu.SelectedMenuPoint = lastSelectedMenuPoint;
            settingsMenu.HandleUsing();
            SaveSettings(true);
            lastSelectedMenuPoint = settingsMenu.SelectedMenuPoint;
            if (settingsMenu.SelectedMenuPoint == 7)
                return;
        }
    }

    private static void ChangeFlushingOption()
        => Storage.ScurSettings.NeedFlushingConsole = !Storage.ScurSettings.NeedFlushingConsole;

    private static void ChangeDataGettingOption()
    {
        Printer.PrintWarning("1 - всегда сохранять в файл, 2 - всегда сохранять в консоль, 3 - всегда спрашивать");
        Storage.ScurSettings.EnterDataChoice = (ConsoleFileOption)InputHandler.GetUserNumberInRange(1, 3);
    }
    
    private static void ChangeWhereToShowResultOption()
    {
        Printer.PrintWarning("1 - всегда сохранять в файл, 2 - всегда сохранять в консоль, 3 - всегда спрашивать");
        Storage.ScurSettings.ShowResultChoice = (ConsoleFileOption)InputHandler.GetUserNumberInRange(1, 3);
    }
    
    private static void ChangeHowToShowResultOption()
    {
        Printer.PrintWarning("1 - в формате JSON, 2 - в формате таблицы, 3 - всегда спрашивать");
        Storage.ScurSettings.ViewingMode = (ViewingMode)InputHandler.GetUserNumberInRange(1, 3);
    }
    
    private static void ChangeInstantOpeningOption()
        => Storage.ScurSettings.NeedOpenFileAfterWriting = !Storage.ScurSettings.NeedOpenFileAfterWriting;
    
    private static void ChangeFavouriteInputFile()
        => Storage.ScurSettings.FavouriteInputFile = InputHandler.GetFilePathToJson("Введите путь до json файла: ");
    
    private static void ChangeFavouriteOutputFile()
        => Storage.ScurSettings.FavouriteOutputFile = InputHandler.GetValidPath("Введите путь до файла, в который нужно сохранять результат: ");
}