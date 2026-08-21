using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class MinInvoicePerCustomerRow
    {
        public int CustomerId { get; set; }

        public decimal MinInvoiceTotal { get; set; }
    }
}
