using System;
using System.Collections.Generic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class CampaignConditionInfo : BaseItem
    {
        public Guid CampaignSystemId { get; set; }
        public string Data { get; set; }
        public Guid SystemId { get; set; }
        public string TypeName { get; set; }
    }
}
