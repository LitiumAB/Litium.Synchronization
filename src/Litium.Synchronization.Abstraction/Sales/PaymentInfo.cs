using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class PaymentInfo : BaseItem
    {
        public List<AdditionalPaymentInfo> AdditionalPaymentInfo { get; set; }
        public Address BillingAddress { get; set; }
        public string ExternalMessage { get; set; }
        public Guid OrderSystemId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentProvider { get; set; }
        public short PaymentStatus { get; set; }
        public List<PaymentInfoRow> Rows { get; set; }
        public string ReferenceId { get; set; }
        public Guid SystemId { get; set; }
        public decimal TotalAmountWithVAT { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionReference { get; set; }
        public decimal VATPercentage { get; set; }
        public decimal VATAmount { get; set; }
    }
}
