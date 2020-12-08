using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class AdditionalDeliveryInfo : BaseItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public Guid SystemId { get; set; }
        public Guid DeliverySystemId { get; set; }
    }
}
