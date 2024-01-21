namespace UILayer;

public class Menu
{
    //TODO: Maybe add selected menuPoint;
    private readonly MenuPoint[] _menuPoints;
    private readonly string?_header;
    
    public int SelectedMenuPoint { get; set; }

    public Menu(MenuPoint[] menuPoints, string header="")
    {
        _menuPoints = menuPoints;
        _header = header;
    }
    
    
    public void Show()
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

    public bool PressMenuPoint(int index)
    {
        if (0 > index || index >= _menuPoints.Length) return false;

        SelectedMenuPoint = index;
        _menuPoints[index].OnMenuPointClick.Invoke();
        return true;
    }


    public void PressSelectedMenuPoint()
        => PressMenuPoint(SelectedMenuPoint);
    
    public void SelectNextMenuPoint() 
        => SelectedMenuPoint = Math.Clamp(SelectedMenuPoint+1, 0, _menuPoints.Length-1);

    public void SelectPreviousMenuPoint()
        => SelectedMenuPoint = Math.Clamp(SelectedMenuPoint-1, 0, _menuPoints.Length-1);

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

    public static Menu CreateChoiceMenu(string[] names)
    {
        MenuPoint[] menuPoints = new MenuPoint[names.Length];
        for (int i = 0; i < names.Length; i++)
            menuPoints[i] = new MenuPoint(names[i]);
        Menu createdMenu = new Menu(menuPoints);

        return createdMenu;
    }
}