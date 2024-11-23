
namespace SimpleRDBMSRestfulAPI.Core;
public class CursorBasedResult
{
    public dynamic? FirstCursor { get; internal set; }
    public dynamic? LastCursor { get; internal set; }
    public List<IDictionary<string, object>> Items { get; internal set; } = new List<IDictionary<string, object>>();
}

public enum CursorDirection
{
    Prev = 0,
    Next = 1
}