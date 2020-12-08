using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class Order : BaseItem
    {
        public List<AdditionalOrderInfo> AdditionalOrderInfo { get; set; }
        public string Comments { get; set; }
        public string CampaignInfo { get; set; }
        public Guid ChannelSystemId { get; set; }
        public string ClientIp { get; set; }
        public string ClientBrowser { get; set; }
        public Guid CountrySystemId { get; set; }
        public Guid CurrencySystemId { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public List<Delivery> Deliveries { get; set; }
        public short DeliveryStatus { get; set; }
        public string ExternalOrderId { get; set; }
        public List<Fee> Fees { get; set; }
        public decimal GrandTotal { get; set; }
        public Guid MarketSystemId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderDiscount> OrderDiscounts { get; set; }
        public List<OrderRow> OrderRows { get; set; }
        public short OrderStatus { get; set; }
        public short OrderType { get; set; }
        public string Origin { get; set; }
        public decimal OverallVatPercentage { get; set; }
        public List<PaymentInfo> PaymentInfo { get; set; }
        public short PaymentStatus { get; set; }
        public Guid? RmaSystemId { get; set; }
        public Guid? SalesOrderSystemId { get; set; }
        public Guid SystemId { get; set; }
        public decimal TotalDeliveryCost { get; set; }
        public decimal TotalDeliveryCostVAT { get; set; }
        public decimal TotalDeliveryCostWithVAT { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalDiscountVAT { get; set; }
        public decimal TotalDiscountWithVAT { get; set; }
        public decimal TotalFee { get; set; }
        public decimal TotalFeeVAT { get; set; }
        public decimal TotalFeeWithVAT { get; set; }
        public decimal TotalOrderRow { get; set; }
        public decimal TotalOrderRowVAT { get; set; }
        public decimal TotalOrderRowWithVAT { get; set; }
        public decimal TotalVAT { get; set; } 
        public Guid WebsiteSystemId { get; set; }
        public Website Website { get; set; }
    }
}
