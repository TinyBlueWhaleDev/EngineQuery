using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class InvoiceSegmentRow
    {
        public int InvoiceId { get; set; }
        public decimal Total { get; set; }
        public string InvoiceSegment { get; set; } = string.Empty;
    }
}
