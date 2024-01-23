namespace Entities;

public abstract class DataType
{
    public static readonly string S_SecretSep = "`(){}{}()`";
    
    public abstract string this[string fieldName] { get; set; }

}