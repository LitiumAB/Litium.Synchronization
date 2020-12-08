using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class AdditionalOrderInfo : BaseItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public Guid SystemId { get; set; }
        public Guid OrderSystemId { get; set; }
    }
}
