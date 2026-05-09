using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation.Models
{
    public sealed class CompiledQueryDefinition
    {
        public required string TableName { get; set; }
        public List<QuerySelectColumnDefinition> SelectDefinitions { get; } = [];
        public List<QueryWhereDefinition> WhereDefinitions { get; } = [];
        public List<QueryOrderingDefinition> OrderingDefinitions { get; } = [];
        public QueryPaginationDefinition Pagination { get; set; } = new();

    }
}
