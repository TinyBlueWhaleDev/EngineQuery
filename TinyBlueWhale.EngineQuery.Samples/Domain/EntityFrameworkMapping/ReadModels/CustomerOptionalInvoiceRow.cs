using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class CustomerOptionalInvoiceRow
    {        
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal? Total { get; set; }
    }
}
