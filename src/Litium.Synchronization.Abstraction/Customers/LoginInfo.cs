using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Customers
{
    [Serializable]
    public class LoginInfo
    {
        public int AccessFailedCount { get; set; }
        public string Id { get; set; }
        public DateTime? LastLockedOutDate { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTime? LockoutEndDate { get; set; }
        public DateTime PasswordDate { get; set; }
        public string PasswordHash { get; set; }
        public DateTime? PasswordExpirationDate { get; set; }
        public string SecurityStamp { get; set; }
        public Guid SystemId { get; set; }
    }
}
