using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Benchmarks.Benchmarks.Models
{
    public sealed class CustomerInvoiceTotal
    {
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
