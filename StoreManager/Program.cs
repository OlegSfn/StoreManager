using System.Globalization;
using DatabaseLayer;
using DataLayer;
using UILayer;

namespace StoreManager;

internal static class Program
{
    private static void Main()
    {
        // /Users/oleg_sfn/Downloads/default.json
        // /Users/oleg_sfn/Downloads/empty.json
        // /Users/oleg_sfn/Downloads/1obj.json
        // /Users/oleg_sfn/Downloads/1objArr.json
        // /Users/oleg_sfn/Downloads/emptyObj.json
        // /Users/oleg_sfn/Downloads/small.json
        
        //string jsonData = File.ReadAllText("/Users/oleg_sfn/Downloads/emptyObj.json");
        //StoreData store = JsonSerializer.Deserialize<StoreData>(jsonData);
        //Console.WriteLine(store);

        double d = double.Parse("123,34", CultureInfo.InvariantCulture);
        
        RouteManager.HandleEnteringProgram();
        while (true)
        {
            try
            {
                RouteManager.CreateMainMenu().HandleUsing();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                InputHandler.WaitForUserInput("Нажмите любую кнопку, чтобы продолжить: ");
            }
        }
    }
}