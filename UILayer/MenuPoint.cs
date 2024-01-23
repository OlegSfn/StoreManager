namespace UILayer;

public sealed class MenuPoint
{
    public string Text { get; }
    public Action OnMenuPointClick { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MenuPoint"/> class.
    /// </summary>
    /// <param name="text">The text of the menu point.</param>
    /// <param name="onMenuPointClick">The optional action to be executed when the menu point is clicked.</param>
    public MenuPoint(string text, Action? onMenuPointClick = null)
    {
        Text = text;
        OnMenuPointClick = onMenuPointClick ?? (() => { });
    }
}