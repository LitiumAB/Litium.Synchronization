using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class Website : BaseItem
    {
        public string Culture { get; set; }
        public string DomainName { get; set; }
        public string Name { get; set; }
        public Guid SystemId { get; set; }
    }
}
