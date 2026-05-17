using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class CustomerEmailFunctionRow
    {
        public int CustomerId { get; set; }
        public string NormalizedEmail { get; set; } = string.Empty;
        public int EmailLength { get; set; }
    }
}
