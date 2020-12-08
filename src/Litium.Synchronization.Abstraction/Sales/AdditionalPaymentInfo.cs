using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class AdditionalPaymentInfo : BaseItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public Guid PaymentInfoSystemId { get; set; }
        public Guid SystemId { get; set; }
    }
}
