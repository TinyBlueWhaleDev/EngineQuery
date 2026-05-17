using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class LatestInvoicePerCustomerRow
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
