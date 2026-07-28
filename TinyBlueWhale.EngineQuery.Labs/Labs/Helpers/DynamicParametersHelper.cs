using Dapper;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Labs.Labs.Helpers
{
    public static class DynamicParametersHelper
    {
        public static DynamicParameters ToDynamicParameters(this GeneratedSqlQuery query)
        {
            var parameters = new DynamicParameters();

            foreach (var parameter in query.Parameters)
            {
                parameters.Add(
                    parameter.Name,
                    parameter.Value);
            }

            return parameters;
        }
    }
}
