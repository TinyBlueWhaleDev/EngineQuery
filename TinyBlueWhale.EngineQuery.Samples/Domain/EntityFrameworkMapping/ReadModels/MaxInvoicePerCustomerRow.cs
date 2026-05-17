using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class MaxInvoicePerCustomerRow
    {
        public int CustomerId { get; set; }
        public decimal MaxInvoiceTotal { get; set; }
    }
}
