using DatabaseLayer;
using Entities;
using UILayer;

namespace DataLayer;

public static class SettingsManager
{
    private static string s_settingsFileName = "settings.json";
    private static Menu s_settingsMenu;
    
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
            Storage.S_CurSettings = (SettingsData)JsonParser.ReadJson<SettingsData>()[0];
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
    public static void LoadDefaultSettings()
    {
        Storage.S_CurSettings = new SettingsData();
        SaveSettings(true);
    }

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

    private static void ChangeDataGettingOption()
    {
        Printer.PrintWarning("1 - всегда сохранять в файл, 2 - всегда сохранять в консоль, 3 - всегда спрашивать");
        Storage.S_CurSettings.EnterDataChoice = (ConsoleFileOption)InputHandler.GetUserNumberInRange(1, 3);
    }
    
    private static void ChangeWhereToShowResultOption()
    {
        Printer.PrintWarning("1 - всегда сохранять в файл, 2 - всегда сохранять в консоль, 3 - всегда спрашивать");
        Storage.S_CurSettings.ShowResultChoice = (ConsoleFileOption)InputHandler.GetUserNumberInRange(1, 3);
    }
    
    private static void ChangeHowToShowResultOption()
    {
        Printer.PrintWarning("1 - в формате JSON, 2 - в формате таблицы, 3 - всегда спрашивать");
        Storage.S_CurSettings.ViewingMode = (ViewingMode)InputHandler.GetUserNumberInRange(1, 3);
    }
    
    private static void ChangeInstantOpeningOption()
        => Storage.S_CurSettings.NeedOpenFileAfterWriting = !Storage.S_CurSettings.NeedOpenFileAfterWriting;
    
    private static void ChangeFavouriteInputFile()
        => Storage.S_CurSettings.FavouriteInputFile = InputHandler.GetFilePathToJson("Введите путь до json файла: ");
    
    private static void ChangeFavouriteOutputFile()
        => Storage.S_CurSettings.FavouriteOutputFile = InputHandler.GetValidPath("Введите путь до файла, в который нужно сохранять результат: ");
}