using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Customers
{
    [Serializable]
    public class Organization : BaseItem
    {
        public List<Address> Addresses { get; set; }
        public DateTime CreateDate { get; set; }
        public string Description { get; set; }
        public Guid FieldTemplateSystemId { get; set; }
        public List<Field> Fields { get; set; }
        public string Id { get; set; }
        public string LegalRegistrationNumber { get; set; }
        public string Name { get; set; }
        public Guid ParentOrganizationSystemId { get; set; }
        public List<Guid> PersonLinkIds { get; set; }
        public Guid SystemId { get; set; }
    }
}
