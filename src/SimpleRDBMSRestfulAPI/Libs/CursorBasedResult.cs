
using MessagePack;

namespace SimpleRDBMSRestfulAPI.Libs;


[MessagePackObject]
public class CursorBasedResult
{
    [MessagePack.Key("id")]
    public dynamic? FirstCursor { get; internal set; }

    [MessagePack.Key("lastCursor")]
    public dynamic? LastCursor { get; internal set; }

    [MessagePack.Key("items")]
    public IEnumerable<IDictionary<string, object>> Items { get; internal set; } = new List<IDictionary<string, object>>();
}
