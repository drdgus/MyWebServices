namespace MyWebServices.Core.Models;

public class MsWordList : MsWordItem
{
    public ListType Type { get; set; }
    public int Depth { get; set; }
    internal string DepthSign { get; init; }
}