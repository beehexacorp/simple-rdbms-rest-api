using TKSoutdoorsparts.Constants;

namespace TKSoutdoorsparts.Models
{
    public class QueryRequestMetadata : IModel
    {
        public required DbType DbType { get; set; }
        public required string Query { get; set; }
        public required Dictionary<string, object> @params { get; set; }
    }
}