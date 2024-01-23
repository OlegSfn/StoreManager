using System.Text;

namespace Entities;

public sealed class DataBlock
{
    public List<PresentationDataType> DataTypes { get; }
    
    private string Name { get; }
    private List<string> Params { get; }

    public DataBlock(string name, List<string> @params, List<PresentationDataType> dataTypes)
    {
        Name = name;
        Params = @params;
        DataTypes = dataTypes;
    }
    
    public override string ToString()
    {
        StringBuilder fullName = new StringBuilder(Name);
        fullName.Append(" (");
        fullName.Append(string.Join(", ", Params));
        fullName.Append(')');
        return fullName.ToString();
    }
    
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