namespace Entities;

public abstract class DataType : ICloneable
{
    public abstract string[] GetFieldNames();
    public object Clone() => MemberwiseClone();
    public abstract string this[string fieldName] { get; set; }
    
    public abstract int CompareTo(DataType dataType, string fieldName);
}