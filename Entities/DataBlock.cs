using System.Text;

namespace Entities;

public class DataBlock
{
    public string Name { get; }
    public List<string> Params { get; }
    public List<DataType> StoresData { get; }

    public DataBlock(string name, List<string> @params, List<DataType> storesData)
    {
        Name = name;
        Params = @params;
        StoresData = storesData;
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
        if (StoresData.Count == 0)
            return "{}";
        
        if (StoresData.Count == 1)
            return StoresData[0].ToString();
        
        StringBuilder sb = new StringBuilder();
        sb.Append($"[{Environment.NewLine}");
        for (int i = 0; i < StoresData.Count; i++)
        {
            sb.Append(StoresData[i]);
            if (i < StoresData.Count - 1)
                sb.Append($",{Environment.NewLine}");
            else
                sb.Append(Environment.NewLine);
        }

        sb.Append(']');

        return sb.ToString();
    }
}