namespace Entities;

/// <summary>
/// Represents an abstract base class for data types.
/// </summary>
public abstract class DataType
{
    public static readonly string S_SecretSep = "`(){}{}()`";
    
    /// <summary>
    /// Gets or sets the value of a field specified by the field name.
    /// </summary>
    /// <param name="fieldName">The name of the field to get or set.</param>
    /// <returns>The value of the specified field.</returns>
    public abstract string this[string fieldName] { get; set; }

}