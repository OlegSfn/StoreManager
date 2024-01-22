namespace Entities;
using System.Collections;
//TODO: Maybe dont need to clone
public class StoreData : DataType
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string[]? Employees { get; set; }
    public string[]? Products { get; set; }

    private readonly string _noValStr = "Нет значения";
    
    public override string this[string fieldName]
    {
        get
        {
            return fieldName switch
            {
                "store_id" => Id.ToString() ?? _noValStr,
                "store_name" => Name ?? _noValStr,
                "location" => Location ?? _noValStr,
                "employees" => Employees != null ? string.Join(',', Employees) : _noValStr,
                "products" => Products != null ? string.Join(',', Products) : _noValStr
            };
        }
        
        set
        {
            switch (fieldName)
            {
                case "store_id":
                    Id = int.Parse(value);
                    break;
                case "store_name":
                    Name = value;
                    break;
                case "location":
                    Location = value;
                    break;
                case "employees":
                    Employees = value.Split(S_secretSep, StringSplitOptions.RemoveEmptyEntries);
                    break;
                case "products":
                    Products = value.Split(S_secretSep, StringSplitOptions.RemoveEmptyEntries);
                    break;
            }
        }
    }

    public override int CompareTo(DataType dataType, string fieldName)
    {
        StoreData storeData = (StoreData)dataType;
        return fieldName switch
        {
            "store_id" => Comparer.DefaultInvariant.Compare(Id, storeData.Id),
            "store_name" => Comparer.DefaultInvariant.Compare(Name, storeData.Name),
            "location" => Comparer.DefaultInvariant.Compare(Location, storeData.Location),
            "employees" => Comparer.DefaultInvariant.Compare(Employees?.Length ?? 0, storeData.Employees?.Length ?? 0),
            "products" => Comparer.DefaultInvariant.Compare(Products?.Length ?? 0, storeData.Products?.Length ?? 0)
        };
    }

    //TODO: Form array string.
    public override string ToString()
    {
        string employeesString = _noValStr, productsString = _noValStr;
        if (Employees != null)
            employeesString = string.Join($",{Environment.NewLine}\t\t", Employees.Select(x => $"\"{x}\""));
        if (Products != null)
            productsString = string.Join($",{Environment.NewLine}\t\t", Products.Select(x => $"\"{x}\""));
        
        return $"{{{Environment.NewLine}" +
               $"\t\"store_id\": {Id.ToString() ?? _noValStr},{Environment.NewLine}" +
               $"\t\"store_name\": \"{Name ?? _noValStr}\",{Environment.NewLine}" +
               $"\t\"location\": \"{Location ?? _noValStr}\",{Environment.NewLine}" +
               $"\t\"employees\": [\n \t\t{employeesString} {Environment.NewLine}\t],{Environment.NewLine}" +
               $"\t\"products\": [\n \t\t{productsString} {Environment.NewLine}\t]{Environment.NewLine}" +
               $"}}";
    }
    
    public override string[] GetFieldNames()
        => new[] { "store_id", "store_name", "location", "employees", "products" };

    public override string[] GetFieldValues()
        => new[] { Id.ToString() ?? _noValStr, Name ?? _noValStr, Location ?? _noValStr, 
            Employees != null ? string.Join($", ", Employees) : _noValStr, 
            Products != null ? string.Join($", ", Products) : _noValStr };
}