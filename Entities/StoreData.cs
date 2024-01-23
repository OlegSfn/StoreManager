namespace Entities;
using System.Collections;

public class StoreData : DataType
{
    private int? _id;
    private string? _name;
    private string? _location;
    private string[]? _employees;
    private string[]? _products;

    private readonly string _noValStr = "Нет значения";
    
    public override string this[string fieldName]
    {
        get
        {
            return fieldName switch
            {
                "store_id" => _id.ToString() ?? _noValStr,
                "store_name" => _name ?? _noValStr,
                "location" => _location ?? _noValStr,
                "employees" => _employees != null ? string.Join(',', _employees) : _noValStr,
                "products" => _products != null ? string.Join(',', _products) : _noValStr,
                _ => throw new ArgumentOutOfRangeException(nameof(fieldName), fieldName, null)
            };
        }
        
        set
        {
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
                    _employees = value.Split(S_SecretSep, StringSplitOptions.RemoveEmptyEntries);
                    break;
                case "products":
                    _products = value.Split(S_SecretSep, StringSplitOptions.RemoveEmptyEntries);
                    break;
            }
        }
    }

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

    public override string ToString()
    {
        string employeesString = _noValStr, productsString = _noValStr;
        if (_employees != null)
            employeesString = string.Join($",{Environment.NewLine}\t\t", _employees.Select(x => $"\"{x}\""));
        if (_products != null)
            productsString = string.Join($",{Environment.NewLine}\t\t", _products.Select(x => $"\"{x}\""));
        
        return $"{{{Environment.NewLine}" +
               $"\t\"store_id\": {_id.ToString() ?? _noValStr},{Environment.NewLine}" +
               $"\t\"store_name\": \"{_name ?? _noValStr}\",{Environment.NewLine}" +
               $"\t\"location\": \"{_location ?? _noValStr}\",{Environment.NewLine}" +
               $"\t\"employees\": [\n \t\t{employeesString} {Environment.NewLine}\t],{Environment.NewLine}" +
               $"\t\"products\": [\n \t\t{productsString} {Environment.NewLine}\t]{Environment.NewLine}" +
               $"}}";
    }
    
    public override string[] GetFieldNames()
        => new[] { "store_id", "store_name", "location", "employees", "products" };

    public override string[] GetFieldValues()
        => new[] { _id.ToString() ?? _noValStr, _name ?? _noValStr, _location ?? _noValStr, 
            _employees != null ? string.Join($", ", _employees) : _noValStr, 
            _products != null ? string.Join($", ", _products) : _noValStr };
}