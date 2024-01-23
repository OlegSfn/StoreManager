using Entities;

namespace DatabaseLayer;

public static class Storage
{
    public static List<DataBlock> S_DataBlocks { get; } = new();
    public static SettingsData S_CurSettings { get; set; }
    public static TextReader S_StandardInput { get; set; }
    public static TextWriter S_StandardOutput { get; set; }


    public static void AddDataBlock(DataBlock dataBlock)
        => S_DataBlocks.Add(dataBlock);
}