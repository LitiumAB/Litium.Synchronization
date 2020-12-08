using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Customers
{
    [Serializable]
    public class Address
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public AddressType AddressType { get; set; }
        public string CareOf { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string HouseExtension { get; set; }
        public string HouseNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string State { get; set; }
        public Guid SystemId { get; set; }
        public string ZipCode { get; set; }
    }

    public enum AddressType 
    {
        Address,
        AlternativeAddress,
        Billing,
        BillingAndDelivery,
        Delivery
    }
}
