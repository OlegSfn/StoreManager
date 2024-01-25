namespace Entities;
using System.Collections;

/// <summary>
/// Represents data for a store and implements the <see cref="PresentationDataType"/> interface.
/// </summary>
public sealed class StoreData : PresentationDataType
{
    private int? _id;
    private string? _name;
    private string? _location;
    private string[]? _employees;
    private string[]? _products;

    private const string NoValStr = "Нет значения"; // If a field is null, it will be replaced later with this string.

    public StoreData(int? id, string? name, string? location, string[]? employees, string[]? products)
    {
        _id = id;
        _name = name;
        _location = location;
        _employees = employees;
        _products = products;
    }

    public StoreData() : this(null, null, null, null, null) { }


    /// <inheritdoc/>
    public override string this[string fieldName]
    {
        get
        {
            return fieldName switch
            {
                "store_id" => _id?.ToString() ?? NoValStr,
                "store_name" => _name ?? NoValStr,
                "location" => _location ?? NoValStr,
                "employees" => _employees != null ? string.Join(',', _employees) : NoValStr,
                "products" => _products != null ? string.Join(',', _products) : NoValStr,
                _ => throw new ArgumentOutOfRangeException(nameof(fieldName), fieldName, null)
            };
        }
        
        set
        {
            if (value is "null" or $"null{SecretSep}")
                return;
            
            switch (fieldName)
            {
                case "store_id":
                    _id = int.Parse(value);
                    break;
                case "store_name":
                    _name = value;
                    break;
                case "location":
                    _location = value;
                    break;
                case "employees":
                    _employees = value.Split(SecretSep, StringSplitOptions.RemoveEmptyEntries);
                    break;
                case "products":
                    _products = value.Split(SecretSep, StringSplitOptions.RemoveEmptyEntries);
                    break;
            }
        }
    }

    /// <inheritdoc/>
    public override int CompareTo(DataType dataType, string fieldName)
    {
        StoreData storeData = (StoreData)dataType;
        return fieldName switch
        {
            "store_id" => Comparer.DefaultInvariant.Compare(_id, storeData._id),
            "store_name" => Comparer.DefaultInvariant.Compare(_name, storeData._name),
            "location" => Comparer.DefaultInvariant.Compare(_location, storeData._location),
            "employees" => Comparer.DefaultInvariant.Compare(_employees?.Length ?? 0, storeData._employees?.Length ?? 0),
            "products" => Comparer.DefaultInvariant.Compare(_products?.Length ?? 0, storeData._products?.Length ?? 0),
            _ => throw new ArgumentOutOfRangeException(nameof(fieldName), fieldName, null)
        };
    }

    /// <summary>
    /// Provide a string representation of the StoreData object.
    /// </summary>
    /// <returns>A string representation of the StoreData object in JSON format.</returns>
    public override string ToString()
    {
        string employeesString = "null", productsString = "null";
        if (_employees != null)
            employeesString = $"[{Environment.NewLine}\t\t{string.Join($",{Environment.NewLine}\t\t", _employees.Select(x => $"\"{x}\""))}{Environment.NewLine}\t]";
        if (_products != null)
            productsString = $"[{Environment.NewLine}\t\t{string.Join($",{Environment.NewLine}\t\t", _products.Select(x => $"\"{x}\""))}{Environment.NewLine}\t]";
        
        return $"{{{Environment.NewLine}" +
               $"\t\"store_id\": {_id?.ToString() ?? "null"},{Environment.NewLine}" +
               $"\t\"store_name\": {(_name != null ? $"\"{_name}\"" : "null")},{Environment.NewLine}" +
               $"\t\"location\": {(_location != null ? $"\"{_location}\"" : "null")},{Environment.NewLine}" +
               $"\t\"employees\": {employeesString},{Environment.NewLine}" +
               $"\t\"products\": {productsString}{Environment.NewLine}" +
               $"}}";
    }
    
    /// <inheritdoc/>
    public override string[] GetFieldNames()
        => new[] { "store_id", "store_name", "location", "employees", "products" };

    /// <inheritdoc/>
    public override string[] GetFieldValues()
        => new[] { _id?.ToString() ?? NoValStr, _name ?? NoValStr, _location ?? NoValStr, 
            _employees != null ? string.Join(", ", _employees) : NoValStr, 
            _products != null ? string.Join(", ", _products) : NoValStr };
}