namespace Entities;

public abstract class DataType : ICloneable
{
    public static readonly string S_secretSep = "`(){}{}()`";
    
    public abstract string[] GetFieldNames();
    public abstract string[] GetFieldValues();
    
    public object Clone() => MemberwiseClone();
    public abstract string this[string fieldName] { get; set; }
    
    public abstract int CompareTo(DataType dataType, string fieldName);

}