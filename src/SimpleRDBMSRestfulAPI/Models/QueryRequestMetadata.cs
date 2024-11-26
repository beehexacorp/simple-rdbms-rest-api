using SimpleRDBMSRestfulAPI.Constants;

namespace SimpleRDBMSRestfulAPI.Models
{
    public class QueryRequestMetadata
    {
        public required string Query { get; set; }
        public required Dictionary<string, object> @params { get; set; }
    }
}
