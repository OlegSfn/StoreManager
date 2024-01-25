using UILayer;

namespace StoreManager;

internal static class Program
{
    private static void Main()
    {
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