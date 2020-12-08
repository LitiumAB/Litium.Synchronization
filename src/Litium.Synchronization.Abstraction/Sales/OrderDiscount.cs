using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class OrderDiscount : BaseItem
    {
        public Guid CampaignSystemId { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountAmountWithVAT { get; set; }
        public string DiscountDescription { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string ExternalCampaignId { get; set; }
        public Guid OrderSystemId { get; set; }
        public Guid? SalesOrderDiscountSystemId { get; set; }
        public Guid SystemId { get; set; }
        public decimal VATAmount { get; set; }
        public decimal VATPercentage { get; set; }
    }
}
