using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class Address : BaseItem
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CareOf { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string FirstName { get; set; }
        public string HouseExtension { get; set; }
        public string HouseNumber { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public string OrganizationName { get; set; }
        public string Phone { get; set; }
        public string State { get; set; }
        public Guid SystemId { get; set; }
        public string Zip { get; set; }
        public string Title { get; set; }
    }
}
