using System.Text;

namespace Entities;

/// <summary>
/// Represents a block of data consisting of presentation data types.
/// </summary>
public sealed class DataBlock
{
    public List<PresentationDataType> DataTypes { get; }
    
    private string Name { get; }
    private List<string> Params { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataBlock"/> class with the specified name, parameters, and data types.
    /// </summary>
    /// <param name="name">The name of the data block.</param>
    /// <param name="params">The parameters associated with the data block.</param>
    /// <param name="dataTypes">The list of presentation data types in the data block.</param>
    public DataBlock(string name, List<string> @params, List<PresentationDataType> dataTypes)
    {
        Name = name;
        Params = @params;
        DataTypes = dataTypes;
    }
    
    /// <summary>
    /// Returns a string representation of the data block.
    /// </summary>
    /// <returns>A formatted string representing the data block.</returns>
    public override string ToString()
    {
        StringBuilder fullName = new StringBuilder(Name);
        fullName.Append(" (");
        fullName.Append(string.Join(", ", Params));
        fullName.Append(')');
        return fullName.ToString();
    }
    
    /// <summary>
    /// Forms and returns a JSON representation of the data block.
    /// </summary>
    /// <returns>A JSON string representing the data block.</returns>
    public string FormJson()
    {
        if (DataTypes.Count == 0)
            return "{}";
        
        if (DataTypes.Count == 1)
            return DataTypes[0].ToString();
        
        StringBuilder sb = new StringBuilder();
        sb.Append($"[{Environment.NewLine}");
        for (int i = 0; i < DataTypes.Count; i++)
        {
            sb.Append(DataTypes[i]);
            if (i < DataTypes.Count - 1)
                sb.Append($",{Environment.NewLine}");
            else
                sb.Append(Environment.NewLine);
        }

        sb.Append(']');

        return sb.ToString();
    }
}