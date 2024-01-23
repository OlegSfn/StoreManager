namespace Entities;

/// <summary>
/// Represents an abstract base class for presentation data types.
/// </summary>
public abstract class PresentationDataType : DataType, ICloneable
{
    /// <summary>
    /// Gets an array of field names for the presentation data type.
    /// </summary>
    /// <returns>An array of field names.</returns>
    public abstract string[] GetFieldNames();
    
    /// <summary>
    /// Gets an array of field values for the presentation data type.
    /// </summary>
    /// <returns>An array of field values.</returns>
    public abstract string[] GetFieldValues();
    
    /// <summary>
    /// Creates a shallow copy of the presentation data type.
    /// </summary>
    /// <returns>A shallow copy of the presentation data type.</returns>
    public object Clone() => MemberwiseClone();
    
    /// <summary>
    /// Compares the current presentation data type to another data type based on the specified field name.
    /// </summary>
    /// <param name="dataType">The data type to compare to.</param>
    /// <param name="fieldName">The name of the field to use for comparison.</param>
    /// <returns>An integer that indicates the relative order of the objects being compared.</returns>
    public abstract int CompareTo(DataType dataType, string fieldName);
}