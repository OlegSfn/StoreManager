﻿using DatabaseLayer;
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

        Storage.SStandardInput = Console.In;
        Storage.SStandardOutput = Console.Out;
        while (true)
        {
            Menu mainMenu = RouteManager.CreateMainMenu();
            try
            {
                mainMenu.HandleUsing();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                InputHandler.WaitForUserInput("Нажмите любую кнопку, чтобы продолжить: ");
            }
        }
    }
}