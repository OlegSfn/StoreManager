namespace Entities;

public abstract class PresentationDataType : DataType, ICloneable
{
    public abstract string[] GetFieldNames();
    public abstract string[] GetFieldValues();
    public object Clone() => MemberwiseClone();
    public abstract int CompareTo(DataType dataType, string fieldName);
}