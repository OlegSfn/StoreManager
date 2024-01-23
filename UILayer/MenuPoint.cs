namespace UILayer;

public sealed class MenuPoint
{
    public string Text { get; }
    public Action OnMenuPointClick { get; }
    
    public MenuPoint(string text, Action? onMenuPointClick = null)
    {
        Text = text;
        OnMenuPointClick = onMenuPointClick ?? (() => { });
    }
}