using System.Diagnostics;
using System.Runtime.InteropServices;
using DatabaseLayer;
using DataLayer;
using Entities;
using UILayer;

namespace StoreManager;

/// <summary>
/// Manages user routes in console application.
/// </summary>
public static class RouteManager
{
    /// <summary>
    /// Handles the setup and initialization of the program upon entering.
    /// </summary>
    public static void HandleEnteringProgram()
    {
        Storage.S_StandardInput = Console.In;
        Storage.S_StandardOutput = Console.Out;
        Storage.S_ExitString =  "Ctrl+Z";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            Storage.S_ExitString = "Ctrl+D";
        
        SettingsManager.LoadSettings();
        if (Storage.S_CurSettings.IsFirstUsing)
            HandleFirstUsing();
    }
    
    /// <summary>
    /// Creates the main menu for the store management application.
    /// </summary>
    /// <returns>The main menu.</returns>
    public static Menu CreateMainMenu()
    {
        List<MenuPoint> menuPoints = new List<MenuPoint>();
        MenuPoint enterData = new MenuPoint("Ввести данные.", EnterData);
        MenuPoint filterData = new MenuPoint("Отфильтровать данные.", FilterData);
        MenuPoint sortData = new MenuPoint("Отсортировать данные.", SortData);
        MenuPoint saveData = new MenuPoint("Сохранить (вывести) данные.", SaveData);
        MenuPoint settings = new MenuPoint("Настройки.", SettingsManager.OpenSettings);
        MenuPoint help = new MenuPoint("Помощь.", OpenHelp);
        MenuPoint exit = new MenuPoint("Выйти.", () => Environment.Exit(0));
        
        menuPoints.Add(enterData);
        if (Storage.S_DataBlocks.Count > 0)
        {
            menuPoints.Add(filterData);
            menuPoints.Add(sortData);
            menuPoints.Add(saveData);
        }
        menuPoints.Add(settings);
        menuPoints.Add(help);
        menuPoints.Add(exit);
        Menu mainMenu = new Menu(menuPoints.ToArray());
        return mainMenu;
    }

    /// <summary>
    /// Handles the first usage of the application by prompting the user for configuration.
    /// </summary>
    private static void HandleFirstUsing()
    {
        const string question = "Вы ни разу не открывали это приложение, хотите настроить его?";
        Storage.S_CurSettings!.IsFirstUsing = false;
        SettingsManager.SaveSettings(true);
        if (InputHandler.AskUserYesOrNo(question, ConsoleKey.Enter))
            SettingsManager.OpenSettings();
        else
        {
            SettingsManager.LoadDefaultSettings();
            Printer.FullClear();
        }
        
        OpenHelp();
    }
    
    /// <summary>
    /// Allows the user to enter data into the application.
    /// </summary>
    private static void EnterData()
    {
        void EnterDataViaConsole()
        {
            Console.WriteLine($"Введите ваши данные (чтобы вернуться в программу в конце нажмите \"{Storage.S_ExitString}\"):");
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
                string? filePath = Storage.S_CurSettings.FavouriteInputFile;
                if (filePath == string.Empty)
                    filePath = InputHandler.GetFilePathToJson($"Введите путь до json файла, из которого надо считать данные или \"{Storage.S_ExitString}\", чтобы выйти: ");
                else if (!File.Exists(filePath))
                {
                    filePath = InputHandler.GetFilePathToJson($"Файла по любимому пути больше нет, пожалуйста, введите путь до json файла, из которого надо считать данные или \"{Storage.S_ExitString}\", чтобы выйти: ");
                    Storage.S_CurSettings.FavouriteInputFile = string.Empty;
                    SettingsManager.SaveSettings(true);
                }
                
                if (filePath == null)
                    return;
                
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    /// <summary>
    /// Filters data based on user input.
    /// </summary>
    private static void FilterData()
    {
        string[] savedData = Storage.S_DataBlocks.Select(x => x.ToString()).ToArray();
        Menu savedMenu = Menu.CreateChoiceMenu(savedData);
        savedMenu.HandleUsing();
        
        PresentationDataType[] dataTypes = Storage.S_DataBlocks[savedMenu.SelectedMenuPoint].DataTypes.ToArray();
        if (dataTypes.Length == 0)
        {
            Printer.PrintWarning("Нет объектов для фильтирации");
            return;
        }
        
        string[] fieldNames = dataTypes[0].GetFieldNames();
        Menu filterMenu = Menu.CreateChoiceMenu(fieldNames);
        filterMenu.HandleUsing();

        string value = InputHandler.SafeReadline("Введите значение для выборки (Не забудьте, что элементы массива разделёны строго одной запятой без пробелов): ", "Введена недопустимая строка.");
        DataBlock dataBlock = DataManager.FilterData(dataTypes, fieldNames[filterMenu.SelectedMenuPoint], value);
        Storage.AddDataBlock(dataBlock);
    }
    
    /// <summary>
    /// Sorts data based on user input.
    /// </summary>
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
    
    /// <summary>
    /// Saves data based on user input.
    /// </summary>
    private static void SaveData()
    {
        string[] saveDatas = Storage.S_DataBlocks.Select(x => x.ToString()).ToArray();
        Menu saveMenu = Menu.CreateChoiceMenu(saveDatas);
        saveMenu.HandleUsing();
        
        void ShowDataViaFile()
        {
            string? filePath = Storage.S_CurSettings.FavouriteOutputFile;
            if (filePath == string.Empty)
                filePath = InputHandler.GetValidPath($"Введите путь, куда требуется сохранить данные или \"{Storage.S_ExitString}\", чтобы выйти: ");
            
            if (filePath == null)
                return;
            
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
                Printer.FullClear();
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
    
    /// <summary>
    /// Opens a file in the default editor.
    /// </summary>
    /// <param name="filePath">The path of the file to open.</param>
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

    /// <summary>
    /// Opens the help menu, providing information on menu navigation and actions.
    /// </summary>
    private static void OpenHelp()
    {
        Printer.FullClear();
        string helpText = $"Меню: управление по меню осуществляется с помощью стрелок вверх-вниз или цифр на клавиатуре, для подтверждения действия нажмите \"Enter\".{Environment.NewLine}" +
                          $"После того, как будут введены корректные данные, программа даст возможность работать с ними.{Environment.NewLine}" +
                          $"В настройках можно выбрать любимые пути до файла, тогда Вам не будет предложено каждый раз выбирать файл, из которого нужно считать или в который нужно вывести данные, а будет взят файл по любимому пути.{Environment.NewLine}" +
                          $"Чтобы сделать выборку по массиву, введите его значения через запятую без пробела.";
        Console.WriteLine(helpText);
        InputHandler.WaitForUserInput("Нажмите любую кнопку, чтобы вернуться в меню: ");
    }
}