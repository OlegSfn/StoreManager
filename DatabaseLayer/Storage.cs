using Entities;

namespace DatabaseLayer;

/// <summary>
/// Provides a static storage class for managing data blocks, settings, standard input, and standard output.
/// </summary>
public static class Storage
{
    public static List<DataBlock> S_DataBlocks { get; } = new();
    public static SettingsData S_CurSettings { get; set; }
    public static TextReader S_StandardInput { get; set; }
    public static TextWriter S_StandardOutput { get; set; }
    
    /// <summary>
    /// Adds a data block to the list of stored data blocks.
    /// </summary>
    /// <param name="dataBlock">The DataBlock to be added to the storage.</param>
    public static void AddDataBlock(DataBlock dataBlock)
        => S_DataBlocks.Add(dataBlock);
}