using DatabaseLayer;
using Entities;
using UILayer;

namespace DataLayer;

public static class DataManager
{
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

    public static DataBlock FilterData(PresentationDataType[] dataTypes, string fieldName, string value)
    {
        List<PresentationDataType> filteredData = new List<PresentationDataType>();
        foreach (PresentationDataType dataType in dataTypes)
        {
            if (dataType[fieldName] == value)
                filteredData.Add((PresentationDataType)dataType.Clone());
        }
        
        return new DataBlock("Выборка", new List<string> {fieldName, value}, filteredData);
    }

    public static DataBlock SortData(PresentationDataType[] dataTypes, string fieldName, bool isReversed)
    {
        int mult = isReversed ? -1 : 1;
        Array.Sort(dataTypes,0,dataTypes.Length, Comparer<PresentationDataType>.Create((data1, data2) => 
            data1.CompareTo(data2, fieldName) * mult));
        return new DataBlock("Сортировка", new List<string> {fieldName, isReversed.ToString()}, dataTypes.ToList());
    }
}