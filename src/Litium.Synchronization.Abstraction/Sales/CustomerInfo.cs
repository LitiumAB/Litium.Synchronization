using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class CustomerInfo : BaseItem
    {
        public Address  Address { get; set; }
        public Guid AddressSystemId { get; set; }
        public string CustomerNumber { get; set; }
        public Guid OrganizationSystemId { get; set; }
        public Guid PersonSystemId { get; set; }
        public Guid SystemId { get; set; }
    }
}
