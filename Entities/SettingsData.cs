namespace Entities;

/// <summary>
/// Represents the options for entering data or showing results in the console or file.
/// </summary>
public enum ConsoleFileOption
{
    AlwaysWithFile = 1,
    AlwaysWithConsole = 2,
    AlwaysAskUser = 3
}

/// <summary>
/// Represents the different modes for viewing data.
/// </summary>
public enum ViewingMode
{
    Json = 1,
    Table = 2,
    AskUser = 3
}

/// <summary>
/// Represents settings data for the application.
/// </summary>
public sealed class SettingsData : DataType
{
    public bool IsFirstUsing { get; set; } = true;
    public bool NeedOpenFileAfterWriting { get; set; } = false;
    
    public string FavouriteInputFile { get; set; } = string.Empty; // done
    public string FavouriteOutputFile { get; set; } = string.Empty; // done
    
    public ConsoleFileOption EnterDataChoice { get; set; } = ConsoleFileOption.AlwaysAskUser;
    public ConsoleFileOption ShowResultChoice { get; set; } = ConsoleFileOption.AlwaysAskUser;

    public ViewingMode ViewingMode { get; set; } = ViewingMode.AskUser;

    /// <summary>
    /// Gets or sets the value of a field specified by the field name.
    /// </summary>
    /// <param name="fieldName">The name of the field to get or set.</param>
    /// <returns>The value of the specified field.</returns>
    public override string this[string fieldName]
    {
        get => "";

        set
        {
            switch (fieldName)
            {
                case "IsFirstUsing":
                    IsFirstUsing = value == "true";
                    break;
                case "NeedOpenFileAfterWriting":
                    NeedOpenFileAfterWriting = value == "true";;
                    break;
                case "FavouriteInputFile":
                    FavouriteInputFile = value;
                    break;
                case "FavouriteOutputFile":
                    FavouriteOutputFile = value;
                    break;
                case "EnterDataChoice":
                    EnterDataChoice = (ConsoleFileOption)int.Parse(value);
                    break;
                case "ShowResultChoice":
                    ShowResultChoice = (ConsoleFileOption)int.Parse(value);
                    break;
                case "ViewingMode":
                    ViewingMode = (ViewingMode)int.Parse(value);
                    break;
            }
        }
    }

    /// <summary>
    /// Returns a string representation of the settings data in JSON format.
    /// </summary>
    /// <returns>A string representing the settings data in JSON format.</returns>
    public override string ToString()
    {
        return $"{{{Environment.NewLine}" +
               $"\t\"IsFirstUsing\": {IsFirstUsing.ToString().ToLower()},{Environment.NewLine}" +
               $"\t\"NeedOpenFileAfterWriting\": {NeedOpenFileAfterWriting.ToString().ToLower()},{Environment.NewLine}" +
               $"\t\"FavouriteInputFile\": \"{FavouriteInputFile.Replace("\\", "\\\\")}\",{Environment.NewLine}" +
               $"\t\"FavouriteOutputFile\": \"{FavouriteOutputFile.Replace("\\", "\\\\")}\",{Environment.NewLine}" +
               $"\t\"EnterDataChoice\": {(int)EnterDataChoice},{Environment.NewLine}" +
               $"\t\"ShowResultChoice\": {(int)ShowResultChoice},{Environment.NewLine}" +
               $"\t\"ViewingMode\": {(int)ViewingMode}{Environment.NewLine}" +
               $"}}";
    }
}