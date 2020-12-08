using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Litium.Synchronization.Abstraction.Sales
{
    [Serializable]
    public class PaymentInfoRow : BaseItem
    {
        public string Description { get; set; }
        public int Index { get; set; }
        public Guid PaymentInfoSystemId { get; set; }
        public decimal Quantity { get; set; }
        public string ReferenceId { get; set; }
        public byte ReferenceType { get; set; }
        public Guid? SalesOrderPaymentInfoRowSystemId { get; set; }
        public Guid SystemId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal VatPercentage { get; set; }
    }
}
