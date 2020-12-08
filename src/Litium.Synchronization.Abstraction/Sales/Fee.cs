using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class Fee : BaseItem
    {
        public decimal Amount { get; set; }
        public decimal AmountWithVAT { get; set; }
        public bool KeepAmountWithVATConstant { get; set; }
        public Guid CampaignSystemId { get; set; }
        public string Description { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Name { get; set; }
        public string ExternalCampaignId { get; set; }
        public Guid OrderSystemId { get; set; }
        public Guid? SalesOrderFeeSystemId { get; set; }
        public Guid SystemId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalVATAmount { get; set; }
        public decimal VATPercentage { get; set; }
    }
}
