namespace Entities;

public enum ConsoleFileOption
{
    AlwaysWithFile = 1,
    AlwaysWithConsole = 2,
    AlwaysAskUser = 3
}

public enum ViewingMode
{
    Json = 1,
    Table = 2,
    AskUser = 3
}

public class SettingsData : DataType
{
    public bool IsFirstUsing { get; set; } = true; // done
    public bool NeedFlushingConsole { get; set; } = true;
    public bool NeedOpenFileAfterWriting { get; set; } = true; // done
    
    public string FavouriteInputFile { get; set; } = string.Empty; // done
    public string FavouriteOutputFile { get; set; } = string.Empty; // done
    
    public ConsoleFileOption EnterDataChoice { get; set; } = ConsoleFileOption.AlwaysAskUser; // done
    public ConsoleFileOption ShowResultChoice { get; set; } = ConsoleFileOption.AlwaysAskUser; // done

    public ViewingMode ViewingMode { get; set; } = ViewingMode.AskUser; // done

    public override string[] GetFieldNames() => Array.Empty<string>();

    public override string[] GetFieldValues() => Array.Empty<string>();

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
                case "NeedFlushingConsole":
                    NeedFlushingConsole = value == "true";;
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

    public override int CompareTo(DataType dataType, string fieldName) => -1;

    public override string ToString()
    {
        return $"{{{Environment.NewLine}" +
               $"\t\"IsFirstUsing\": {IsFirstUsing.ToString().ToLower()},{Environment.NewLine}" +
               $"\t\"NeedFlushingConsole\": {NeedFlushingConsole.ToString().ToLower()},{Environment.NewLine}" +
               $"\t\"NeedOpenFileAfterWriting\": {NeedOpenFileAfterWriting.ToString().ToLower()},{Environment.NewLine}" +
               $"\t\"FavouriteInputFile\": \"{FavouriteInputFile}\",{Environment.NewLine}" +
               $"\t\"FavouriteOutputFile\": \"{FavouriteOutputFile}\",{Environment.NewLine}" +
               $"\t\"EnterDataChoice\": {(int)EnterDataChoice},{Environment.NewLine}" +
               $"\t\"ShowResultChoice\": {(int)ShowResultChoice},{Environment.NewLine}" +
               $"\t\"ViewingMode\": {(int)ViewingMode},{Environment.NewLine}" +
               $"}}";
    }
}