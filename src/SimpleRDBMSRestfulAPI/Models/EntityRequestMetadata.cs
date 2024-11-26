using SimpleRDBMSRestfulAPI.Constants;

namespace SimpleRDBMSRestfulAPI.Models;

public class EntityRequestMetadata
{
    public required string TableName { get; set; }
    public required Dictionary<string, object> @params { get; set; }
    public IEnumerable<string>? Fields { get; set; }
    public IEnumerable<string>? Conditions { get; set; }
    public string? OrderBy { get; set; }
}
