using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping
{
    public sealed class CategoryEf
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
