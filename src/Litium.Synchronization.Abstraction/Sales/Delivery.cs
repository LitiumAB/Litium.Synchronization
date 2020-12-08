using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class Delivery : BaseItem
    {
        public DateTime ActualDeliveryDate { get; set; }
        public List<AdditionalDeliveryInfo> AdditionalDeliveryInfo { get; set; }
        public Address Address { get; set; }
        public Guid CampaignSystemId { get; set; }
        public string Comments { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal DeliveryCostWithVAT { get; set; }
        public Guid DeliveryMethodSystemId { get; set; }
        public Guid DeliveryProviderSystemId { get; set; }
        public short DeliveryStatus { get; set; }
        public decimal DiscountAmount { get; set; }
        public string ExternalCampaignId { get; set; }
        public string ExternalReferenceId { get; set; }
        public bool IsCustomDeliveryCost { get; set; }
        public bool IsSentToDeliveryProvider { get; set; }
        public bool KeepDeliveryCostWithVatConstant { get; set; }
        public Guid OrderSystemId { get; set; }
        public DateTime PromisedDeliveryDate { get; set; }
        public Guid? SalesOrderDeliverySystemId { get; set; }
        public Guid SystemId { get; set; }
        public decimal TotalDeliveryCost { get; set; }
        public decimal TotalDeliveryCostWithVat { get; set; }
        public decimal TotalVATAmount { get; set; }
        public string TrackingUrl { get; set; }
        public decimal VATPercentage { get; set; }
    }
}
