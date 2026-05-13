using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Playground.Models
{
    public sealed class JoinUser
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }
    }

    public sealed class JoinOrder
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public decimal Total { get; set; }
    }

    public sealed class JoinOrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int Quantity { get; set; }
    }
}
