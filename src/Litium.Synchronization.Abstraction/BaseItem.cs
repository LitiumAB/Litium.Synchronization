using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction
{
    [Serializable]
    public abstract class BaseItem
    {
        public DateTime LastUpdatedDate { get; set; }
        public Guid LastUpdatedUserSystemId { get; set; }

    }
}
