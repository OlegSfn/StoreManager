using Entities;

namespace DatabaseLayer;

public static class Storage
{
    public static List<DataBlock> SdataBlocks { get; set; } = new();
    public static int SCurDataBlockIndex { get; set; } = 0;
    public static SettingsData ScurSettings { get; set; }
    public static TextReader SStandardInput { get; set; }
    public static TextWriter SStandardOutput { get; set; }


    public static void AddDataBlock(DataBlock dataBlock)
        => SdataBlocks.Add(dataBlock);
}