using DatabaseLayer;
using Entities;
using UILayer;

namespace DataLayer;

/// <summary>
/// Provides functionality for managing and processing data.
/// </summary>
public static class DataManager
{
    /// <summary>
    /// Reads data from a JSON file and adds it to the storage as a new data block.
    /// </summary>
    /// <param name="filePath">The path of the JSON file to read data from. Default is "Консоль".</param>
    public static void EnterData(string filePath = "Консоль")
    {
        List<StoreData> storesData = JsonParser.ReadJson<StoreData>();
        if (storesData.Count == 0)
        {
            Printer.PrintWarning("В введённых данных не нашлось ни одного объекта.");
            InputHandler.WaitForUserInput("Нажмите любую кнопку, чтобы продолжить: ");
            return;
        }
        DataBlock dataBlock = new DataBlock("Исходные данные", new List<string>{Path.GetFileName(filePath)}, storesData.Cast<PresentationDataType>().ToList());
        Storage.S_DataBlocks.Add(dataBlock);
    }

    /// <summary>
    /// Filters data based on the specified field name and value.
    /// </summary>
    /// <param name="dataTypes">The array of PresentationDataType to filter.</param>
    /// <param name="fieldName">The name of the field to filter by.</param>
    /// <param name="value">The value to filter for in the specified field.</param>
    /// <returns>A new data block containing the filtered data.</returns>
    public static DataBlock FilterData(IEnumerable<PresentationDataType> dataTypes, string fieldName, string value)
    {
        List<PresentationDataType> filteredData = new List<PresentationDataType>();
        foreach (PresentationDataType dataType in dataTypes)
        {
            if (dataType[fieldName] == value)
                filteredData.Add((PresentationDataType)dataType.Clone());
        }
        
        return new DataBlock("Выборка", new List<string> {fieldName, $"по \"{value}\""}, filteredData);
    }

    /// <summary>
    /// Sorts data based on the specified field name and sorting order.
    /// </summary>
    /// <param name="dataTypes">The array of PresentationDataType to sort.</param>
    /// <param name="fieldName">The name of the field to sort by.</param>
    /// <param name="isReversed">Indicates whether the sorting order is reversed.</param>
    /// <returns>A new data block containing the sorted data.</returns>
    public static DataBlock SortData(PresentationDataType[] dataTypes, string fieldName, bool isReversed)
    {
        int mult = isReversed ? -1 : 1;
        Array.Sort(dataTypes,0,dataTypes.Length, Comparer<PresentationDataType>.Create((data1, data2) => 
            data1.CompareTo(data2, fieldName) * mult));
        return new DataBlock("Сортировка", new List<string> {fieldName, isReversed ? "в порядке убывания" : "в порядке возрастания"}, dataTypes.ToList());
    }
}