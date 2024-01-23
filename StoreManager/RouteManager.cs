using System.Diagnostics;
using DatabaseLayer;
using DataLayer;
using Entities;
using UILayer;

namespace StoreManager;

public static class RouteManager
{
    public static Menu CreateMainMenu()
    {
        List<MenuPoint> menuPoints = new List<MenuPoint>();
        MenuPoint enterData = new MenuPoint("Ввести данные.", EnterData);
        MenuPoint filterData = new MenuPoint("Отфильтровать данные.", FilterData);
        MenuPoint sortData = new MenuPoint("Отсортировать данные.", SortData);
        MenuPoint saveData = new MenuPoint("Сохранить (вывести) данные.", SaveData);
        MenuPoint settings = new MenuPoint("Настройки.", SettingsManager.OpenSettings);
        MenuPoint exit = new MenuPoint("Выйти.", () => Environment.Exit(0));
        
        menuPoints.Add(enterData);
        if (Storage.S_DataBlocks != null && Storage.S_DataBlocks.Count > 0)
        {
            menuPoints.Add(filterData);
            menuPoints.Add(sortData);
            menuPoints.Add(saveData);
        }
        menuPoints.Add(settings);
        menuPoints.Add(exit);
        Menu mainMenu = new Menu(menuPoints.ToArray());
        return mainMenu;
    }

    public static void HandleFirstUsing()
    {
        string question = "Вы ни разу не открывали это приложение, хотите настроить его?";
        Storage.S_CurSettings!.IsFirstUsing = false;
        SettingsManager.SaveSettings(true);
        if (InputHandler.AskUserYesOrNo(question, ConsoleKey.Enter))
            SettingsManager.OpenSettings();
        else
        {
            SettingsManager.LoadDefaultSettings();
            Printer.FullClear();
        }
    }
    
    private static void EnterData()
    {
        void EnterDataViaConsole()
        {
            Console.WriteLine($"Введите ваши данные (чтобы вернуться в программу в конце напишите \"{JsonParser.SExitString}\"):");
            try
            {
                DataManager.EnterData();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                InputHandler.WaitForUserInput("Нажмите любую кнопку, чтобы продолжить: ");
            }
        }

        void EnterDataViaFile()
        {
            try
            {
                string filePath = Storage.S_CurSettings.FavouriteInputFile;
                if (filePath == string.Empty)
                    filePath = InputHandler.GetFilePathToJson("Введите путь до json файла, из которого надо считать данные: ");
                
                using StreamReader sr = new StreamReader(filePath);
                Console.SetIn(sr);
                DataManager.EnterData(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                InputHandler.WaitForUserInput("Нажмите любую кнопку, чтобы продолжить: ");
            }
            finally
            {
                Console.SetIn(Storage.S_StandardInput);
            }
        }

        switch (Storage.S_CurSettings.EnterDataChoice)
        {
            case ConsoleFileOption.AlwaysAskUser:
            {
                MenuPoint fileMode = new MenuPoint("Считать из файла");
                MenuPoint consoleMode = new MenuPoint("Считать из консоли");
                Menu chooseModeMenu = new Menu(new[] { fileMode, consoleMode });
                chooseModeMenu.HandleUsing();
                if (chooseModeMenu.SelectedMenuPoint == 0)
                    EnterDataViaFile();
                else
                    EnterDataViaConsole();
                break;
            }
            case ConsoleFileOption.AlwaysWithConsole:
                EnterDataViaConsole();
                break;
            case ConsoleFileOption.AlwaysWithFile:
                EnterDataViaFile();
                break;
        }
    }
    
    private static void FilterData()
    {
        string[] savedData = Storage.S_DataBlocks.Select(x => x.ToString()).ToArray();
        Menu savedMenu = Menu.CreateChoiceMenu(savedData);
        savedMenu.HandleUsing();
        
        //TODO: check 2 arrays
        PresentationDataType[] dataTypes = Storage.S_DataBlocks[savedMenu.SelectedMenuPoint].DataTypes.ToArray();
        if (dataTypes.Length == 0)
        {
            Printer.PrintWarning("Нет объектов для фильтирации");
            return;
        }
        
        string[] fieldNames = dataTypes[0].GetFieldNames();
        Menu filterMenu = Menu.CreateChoiceMenu(fieldNames);
        filterMenu.HandleUsing();

        string value = InputHandler.SafeReadline("Введите значение для выборки: ", "Введена недопустимая строка.");
        DataBlock dataBlock = DataManager.FilterData(dataTypes, fieldNames[filterMenu.SelectedMenuPoint], value);
        Storage.AddDataBlock(dataBlock);
    }
    
    private static void SortData()
    {
        string[] savedData = Storage.S_DataBlocks.Select(x => x.ToString()).ToArray();
        Menu savedMenu = Menu.CreateChoiceMenu(savedData);
        savedMenu.HandleUsing();
        
        PresentationDataType[] dataTypes = Storage.S_DataBlocks[savedMenu.SelectedMenuPoint].DataTypes.ToArray();
        if (dataTypes.Length == 0)
        {
            Printer.PrintWarning("Нет объектов для сортировки");
            return;
        }
        string[] fieldNames = dataTypes[0].GetFieldNames();
        Menu sortMenu = Menu.CreateChoiceMenu(fieldNames);
        sortMenu.HandleUsing();
        
        MenuPoint increasingOrder = new MenuPoint("Отсортировать по возрастанию");
        MenuPoint descendingOrder = new MenuPoint("Отсортировать по убыванию"); //TODO: check eng
        Menu sortOrderMenu = new Menu(new[] { increasingOrder, descendingOrder });
        sortOrderMenu.HandleUsing();

        DataBlock sortedDataBlock = DataManager.SortData(dataTypes, fieldNames[sortMenu.SelectedMenuPoint], sortOrderMenu.SelectedMenuPoint != 0);
        Storage.AddDataBlock(sortedDataBlock);
    }
    
    private static void SaveData()
    {
        string[] saveDatas = Storage.S_DataBlocks.Select(x => x.ToString()).ToArray();
        Menu saveMenu = Menu.CreateChoiceMenu(saveDatas);
        saveMenu.HandleUsing();
        
        void ShowDataViaFile()
        {
            string filePath = Storage.S_CurSettings.FavouriteOutputFile;
            if (filePath == string.Empty)
                filePath = InputHandler.GetValidPath("Введите путь, куда требуется сохранить данные: ");
            
            using StreamWriter sr = new StreamWriter(filePath);
            Console.SetOut(sr);
            try
            {
                JsonParser.WriteJson(Storage.S_DataBlocks[saveMenu.SelectedMenuPoint]);
                if (Storage.S_CurSettings.NeedOpenFileAfterWriting)
                    OpenFileInEditor(filePath);
            }
            finally
            {
                Console.SetOut(Storage.S_StandardOutput);
            }

            Printer.PrintInfo("Данные успешно записаны!");
        }
        
        void ShowDataViaConsole()
        {
            if (Storage.S_CurSettings.ViewingMode == ViewingMode.AskUser)
            {
                MenuPoint jsonMode = new MenuPoint("В JSON формате ");
                MenuPoint tableMode = new MenuPoint("В табличном формате");
                Menu chooseViewModeMenu = new Menu(new[] { jsonMode, tableMode });
                chooseViewModeMenu.HandleUsing();
                if (chooseViewModeMenu.SelectedMenuPoint == 0)
                {
                    JsonParser.WriteJson(Storage.S_DataBlocks[saveMenu.SelectedMenuPoint]);
                    InputHandler.WaitForUserInput("Нажмите любую кнопку, чтобы продолжить: ");
                }
                else
                    Printer.ShowTable(Storage.S_DataBlocks[saveMenu.SelectedMenuPoint].DataTypes.ToArray());
            }
            else if (Storage.S_CurSettings.ViewingMode == ViewingMode.Json)
            {
                JsonParser.WriteJson(Storage.S_DataBlocks[saveMenu.SelectedMenuPoint]);
                InputHandler.WaitForUserInput("Нажмите любую кнопку, чтобы продолжить: ");
            }
            else
                Printer.ShowTable(Storage.S_DataBlocks[saveMenu.SelectedMenuPoint].DataTypes.ToArray());
        }

        switch (Storage.S_CurSettings.ShowResultChoice)
        {
            case ConsoleFileOption.AlwaysAskUser:
            {
                MenuPoint fileMode = new MenuPoint("Записать в файл");
                MenuPoint consoleMode = new MenuPoint("Вывести в консоль");
                Menu chooseModeMenu = new Menu(new[] { fileMode, consoleMode });
                chooseModeMenu.HandleUsing();
                if (chooseModeMenu.SelectedMenuPoint == 0)
                    ShowDataViaFile();
                else
                    ShowDataViaConsole();
                break;
            }
            case ConsoleFileOption.AlwaysWithFile:
                ShowDataViaFile();
                break;
            case ConsoleFileOption.AlwaysWithConsole:
                ShowDataViaConsole();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
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
}