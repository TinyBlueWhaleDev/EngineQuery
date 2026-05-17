using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping
{
    public sealed class InvoiceEf
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public decimal Total { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
