using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Tests.TestModels
{
    public sealed class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
