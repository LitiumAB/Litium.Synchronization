using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Customers
{
    [Serializable]
    public class Person : BaseItem
    {
        // public bool? AccountIsLockedOut { get; set; }
        //public bool? AccountMustChangePassword { get; set; }
        public List<Address> Addresses { get; set; }
        public List<AssignedRole> AssignedRoles { get; set; }
        public string Comments { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public Guid FieldTemplateSystemId { get; set; }
        public List<Field> Fields { get; set; }
        public string FirstName { get; set; }
        public List<Guid> GroupLinkIds { get; set; }
        public string Id { get; set; }
        public bool IsFemale { get; set; }
        public bool IsMale { get; set; }
        public string LastName { get; set; }
        public LoginInfo LoginInfo { get; set; }
        public List<Guid> OrganizationLinkIds { get; set; }
        public string PhoneHome { get; set; }
        public string PhoneMobile { get; set; }
        public string PhoneWork { get; set; }
        public string Title { get; set; }
        public Guid SystemId { get; set; }
    }
}
