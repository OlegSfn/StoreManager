namespace UILayer;

/// <summary>
/// Represents a menu with multiple menu points.
/// </summary>
public class Menu
{
    private readonly MenuPoint[] _menuPoints;
    private readonly string?_header;
    
    public int SelectedMenuPoint { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Menu"/> class.
    /// </summary>
    /// <param name="menuPoints">The array of menu points.</param>
    /// <param name="header">The optional header text for the menu.</param>
    public Menu(MenuPoint[] menuPoints, string header="")
    {
        _menuPoints = menuPoints;
        _header = header;
    }
    
    /// <summary>
    /// Handles the user's interactions with the menu.
    /// </summary>
    public void HandleUsing()
    {
        Show();
        while (true)
        {
            ConsoleKey pressedKey = Console.ReadKey(true).Key;
            if (char.IsDigit((char)pressedKey))
            {
                if (PressMenuPoint(pressedKey - ConsoleKey.D1))
                    return;
            }

            switch (pressedKey)
            {
                case ConsoleKey.UpArrow:
                    SelectPreviousMenuPoint();
                    Show();
                    break;
                case ConsoleKey.DownArrow:
                    SelectNextMenuPoint();
                    Show();
                    break;
                case ConsoleKey.Enter:
                    PressSelectedMenuPoint();
                    return;
            }
        }
    }

    /// <summary>
    /// Creates a menu with menu points based on the provided names.
    /// </summary>
    /// <param name="names">An array of menu point names.</param>
    /// <returns>The created menu.</returns>
    public static Menu CreateChoiceMenu(string[] names)
    {
        MenuPoint[] menuPoints = new MenuPoint[names.Length];
        for (int i = 0; i < names.Length; i++)
            menuPoints[i] = new MenuPoint(names[i]);
        Menu createdMenu = new Menu(menuPoints);

        return createdMenu;
    }
    
    /// <summary>
    /// Displays the menu on the console.
    /// </summary>
    private void Show()
    {
        Printer.FullClear();
        if (!string.IsNullOrEmpty(_header))
            Console.WriteLine(_header);
        
        for (int i = 0; i < _menuPoints.Length; i++)
        {
            if (i == SelectedMenuPoint)
                Console.WriteLine($"{i+1}. {_menuPoints[i].Text}\t<----");
            else
                Console.WriteLine($"{i+1}. {_menuPoints[i].Text}");
        }
    }

    /// <summary>
    /// Handles pressing a menu point at the specified index.
    /// </summary>
    /// <param name="index">The index of the pressed menu point.</param>
    /// <returns>True if the menu point was successfully pressed, false otherwise.</returns>
    private bool PressMenuPoint(int index)
    {
        if (0 > index || index >= _menuPoints.Length) return false;

        SelectedMenuPoint = index;
        _menuPoints[index].OnMenuPointClick.Invoke();
        return true;
    }
    
    /// <summary>
    /// Presses the currently selected menu point.
    /// </summary>
    private void PressSelectedMenuPoint()
        => PressMenuPoint(SelectedMenuPoint);

    /// <summary>
    /// Selects the next menu point.
    /// </summary>
    private void SelectNextMenuPoint() 
        => SelectedMenuPoint = Math.Clamp(SelectedMenuPoint+1, 0, _menuPoints.Length-1);

    /// <summary>
    /// Selects the previous menu point.
    /// </summary>
    private void SelectPreviousMenuPoint()
        => SelectedMenuPoint = Math.Clamp(SelectedMenuPoint-1, 0, _menuPoints.Length-1);
}