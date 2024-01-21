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
        MenuPoint settings = new MenuPoint("Настройки.", () =>
        {
            Console.WriteLine("Настройки ещё в процессе разработки.");
            InputHandler.WaitForUserInput("Нажмите любую клавишу, чтобы продолжить: ");
        });
        MenuPoint exit = new MenuPoint("Выйти.", () => Environment.Exit(0));
        
        menuPoints.Add(enterData);
        if (Storage.SdataBlocks != null && Storage.SdataBlocks.Count > 0)
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

    private static void EnterData()
    {
        MenuPoint fileMode = new MenuPoint("Считать из файла");
        MenuPoint consoleMode = new MenuPoint("Считать из консоли");
        Menu chooseModeMenu = new Menu(new[] { fileMode, consoleMode });
        chooseModeMenu.HandleUsing();
        
        if (chooseModeMenu.SelectedMenuPoint == 0)
        {
            string filePath = InputHandler.GetFilePathToJson("Введите путь до json файла, из которого надо считать данные: ");
            using StreamReader sr = new StreamReader(filePath);
            Console.SetIn(sr);
            DataManager.EnterData(filePath);
            Console.SetIn(Storage.SStandardInput);
        }
        else
        {
            Console.WriteLine("Введите ваши данные:");
            DataManager.EnterData();
        }
    }
    
    private static void FilterData()
    {
        string[] savedData = Storage.SdataBlocks.Select(x => x.ToString()).ToArray();
        Menu savedMenu = Menu.CreateChoiceMenu(savedData);
        savedMenu.HandleUsing();
        
        //TODO: check 2 arrays
        DataType[] dataTypes = Storage.SdataBlocks[savedMenu.SelectedMenuPoint].StoresData.ToArray();
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
        string[] savedData = Storage.SdataBlocks.Select(x => x.ToString()).ToArray();
        Menu savedMenu = Menu.CreateChoiceMenu(savedData);
        savedMenu.HandleUsing();
        
        DataType[] dataTypes = Storage.SdataBlocks[savedMenu.SelectedMenuPoint].StoresData.ToArray();
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
        string[] saveDatas = Storage.SdataBlocks.Select(x => x.ToString()).ToArray();
        Menu saveMenu = Menu.CreateChoiceMenu(saveDatas);
        saveMenu.HandleUsing();
        
        MenuPoint fileMode = new MenuPoint("Записать в файл");
        MenuPoint consoleMode = new MenuPoint("Вывести в консоль");
        Menu chooseModeMenu = new Menu(new[] { fileMode, consoleMode });
        chooseModeMenu.HandleUsing();
        
        if (chooseModeMenu.SelectedMenuPoint == 0)
        {
            string filePath = InputHandler.GetValidPath("Введите путь, куда требуется сохранить данные: ");
            using StreamWriter sr = new StreamWriter(filePath);
            Console.SetOut(sr);
            JsonParser.WriteJson(Storage.SdataBlocks[saveMenu.SelectedMenuPoint]);
            Console.SetOut(Storage.SStandardOutput);
            Printer.PrintInfo("Данные успешно записаны!");
        }
        else
            JsonParser.WriteJson(Storage.SdataBlocks[saveMenu.SelectedMenuPoint]);
        InputHandler.WaitForUserInput("Нажмите любую кнопку, чтобы продолжить: ");
    }
}