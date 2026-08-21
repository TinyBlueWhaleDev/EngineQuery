using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.FluentMapping
{
    public sealed class CategoryFluent
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
