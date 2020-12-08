using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Customers
{
    [Serializable]
    public class AssignedRole
    {
        public Guid OrganizationSystemId { get; set; }
        public Guid PersonSystemId { get; set; }
        public Guid RoleSystemId { get; set; }
    }
}
