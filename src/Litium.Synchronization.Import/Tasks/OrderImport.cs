using AutoMapper;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Foundation.Security;
using Litium.Globalization;
using Litium.Studio.FieldFramework.FieldConverters;
using Litium.Synchronization.Abstraction;
using Litium.Synchronization.Abstraction.Sales;
using Litium.Websites;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Synchronization.Import.Tasks
{
    public class OrderImport : DefaultImport
    {
        private readonly SecurityToken _adminToken;
        private readonly ModuleECommerce _moduleECommerce;
        private readonly ChannelService _channelService;
        private readonly WebsiteService _websiteService;

        public OrderImport(ChannelService channelService, WebsiteService websiteService) : base("Order") 
        {
            _moduleECommerce = ModuleECommerce.Instance;
            _adminToken = _moduleECommerce.AdminToken;
            _channelService = channelService;
            _websiteService = websiteService;
        }

        protected override BaseItem DeserializeItem(TextReader tr, JsonSerializer serializer)
        {
            return serializer.Deserialize(tr, typeof(Order)) as Order;
        }

        protected override void ProcessItem(BaseItem item)
        {
            var importItem = item as Order;
            if (importItem != null)
            {
                var orderCarrier = Mapper.Map(importItem, typeof(Order), typeof(OrderCarrier)) as OrderCarrier;
                if (orderCarrier != null)
                {
                    SetGlobalization(orderCarrier, importItem);
                    var order = _moduleECommerce.Orders.GetOrder(orderCarrier.ID, _adminToken);
                    if (order == null)
                    {
                        order = _moduleECommerce.Orders.CreateOrder(orderCarrier, _adminToken);
                    }
                    if (order != null)
                    {
                        var currentOrderCarrier = order.GetAsCarrier(true, true, true, true, true, true);
                        SetCarrierStatesToUpdate(orderCarrier, currentOrderCarrier);
                        order.SetValuesFromCarrier(orderCarrier, _adminToken);
                        // order.GetType().GetTypeInfo().GetDeclaredMethod("UpdateFromCarrier").Invoke(order, new object[] { orderCarrier, _adminToken });
                    }
                    else
                    {
                        throw new Exception(string.Format(Messages.OrderUpdateCreateError, orderCarrier.ExternalOrderID));
                    }
                }
                else
                {
                    if (orderCarrier == null)
                    {
                        throw new System.Exception(string.Format(Messages.OrganizationMappingError, importItem.ExternalOrderId));
                    }
                }
            }
        }

        private void SetGlobalization(OrderCarrier orderCarrier, Order importItem)
        {
            if (importItem.Website != null)
            {
                var channel = _channelService.GetAll().FirstOrDefault(x => x.WebsiteSystemId.HasValue && x.WebsiteSystemId.Value == importItem.Website.SystemId && x.WebsiteLanguageSystemId.Value.GetLanguage().CultureInfo.Name.Equals(importItem.Website.Culture, StringComparison.OrdinalIgnoreCase));
                if (channel != null)
                {
                    if (channel.MarketSystemId.HasValue)
                    {
                        orderCarrier.MarketID = channel.MarketSystemId.Value;
                    }
                    orderCarrier.ChannelID = channel.SystemId;
                    var countrySystemId = channel.CountryLinks?.FirstOrDefault()?.CountrySystemId;
                    if (countrySystemId.HasValue)
                    {
                        orderCarrier.CountryID = countrySystemId.Value;
                    }
                }
            }
        }

        private void SetCarrierStatesToUpdate(OrderCarrier orderCarrier, OrderCarrier currentOrderCarrier)
        {
            // Order carrier
            SetCarrierStateToUpdate(orderCarrier.CarrierState);
            orderCarrier.LastUpdatedDate = currentOrderCarrier.LastUpdatedDate;

            // Cusomer info carrier
            SetCarrierStateToUpdate(orderCarrier.CustomerInfo.CarrierState);
            if (currentOrderCarrier.CustomerInfo != null)
            {
                orderCarrier.CustomerInfo.LastUpdatedDate = currentOrderCarrier.CustomerInfo.LastUpdatedDate;
            }

            // Customer info address carrier
            SetCarrierStateToUpdate(orderCarrier.CustomerInfo.Address.CarrierState);
            orderCarrier.CustomerInfo.Address.LastUpdatedDate = currentOrderCarrier.CustomerInfo.Address.LastUpdatedDate;
            foreach (var additionalOrderInfoCarrier in orderCarrier.AdditionalOrderInfo)
            {
                var currentAdditionalOrderInfoCarrier = currentOrderCarrier.AdditionalOrderInfo?.FirstOrDefault(x => x.ID == additionalOrderInfoCarrier.ID);
                SetCarrierStateToUpdate(additionalOrderInfoCarrier.CarrierState);
                if (currentAdditionalOrderInfoCarrier != null)
                {
                    additionalOrderInfoCarrier.LastUpdatedDate = currentAdditionalOrderInfoCarrier.LastUpdatedDate;
                }
            }
            foreach (var orderDiscountCarrier in orderCarrier.OrderDiscounts)
            {
                var currentOrderDiscountCarrier = currentOrderCarrier.OrderDiscounts?.FirstOrDefault(x => x.ID == orderDiscountCarrier.ID);
                SetCarrierStateToUpdate(orderDiscountCarrier.CarrierState);
                if (currentOrderDiscountCarrier != null)
                {
                    orderDiscountCarrier.LastUpdatedDate = currentOrderDiscountCarrier.LastUpdatedDate;
                }
            }
            foreach (var orderRowCarrier in orderCarrier.OrderRows)
            {
                var currentOrderRowCarrier = currentOrderCarrier.OrderRows?.FirstOrDefault(x => x.ID == orderRowCarrier.ID);
                SetCarrierStateToUpdate(orderRowCarrier.CarrierState);
                if (currentOrderRowCarrier != null)
                {
                    orderRowCarrier.LastUpdatedDate = currentOrderRowCarrier.LastUpdatedDate;
                }
            }
            foreach (var deliveryCarrier in orderCarrier.Deliveries)
            {
                var currentDeliveryCarrier = currentOrderCarrier.Deliveries?.FirstOrDefault(x => x.ID == deliveryCarrier.ID);
                SetCarrierStateToUpdate(deliveryCarrier.CarrierState);
                if (currentDeliveryCarrier != null)
                {
                    deliveryCarrier.LastUpdatedDate = currentDeliveryCarrier.LastUpdatedDate;
                }
                foreach (var additionalDeliveryInfoCarrier in deliveryCarrier.AdditionalDeliveryInfo)
                {
                    var currentAdditionalDeliveryInfoCarrier = currentDeliveryCarrier?.AdditionalDeliveryInfo?.FirstOrDefault(x => x.ID == additionalDeliveryInfoCarrier.ID);
                    SetCarrierStateToUpdate(additionalDeliveryInfoCarrier.CarrierState);
                    if (currentAdditionalDeliveryInfoCarrier != null)
                    {
                        additionalDeliveryInfoCarrier.LastUpdatedDate = currentAdditionalDeliveryInfoCarrier.LastUpdatedDate;
                    }
                }

                SetCarrierStateToUpdate(deliveryCarrier.Address.CarrierState);
                if (currentDeliveryCarrier?.Address != null)
                {
                    deliveryCarrier.Address.LastUpdatedDate = currentDeliveryCarrier.Address.LastUpdatedDate;
                }
            }
            foreach (var paymentInfoCarrier in orderCarrier.PaymentInfo)
            {
                var currentPaymentInfoCarrier = currentOrderCarrier.PaymentInfo?.FirstOrDefault(x => x.ID == paymentInfoCarrier.ID);
                SetCarrierStateToUpdate(paymentInfoCarrier.CarrierState);
                if (currentPaymentInfoCarrier != null)
                {
                    paymentInfoCarrier.LastUpdatedDate = currentPaymentInfoCarrier.LastUpdatedDate;
                }
                foreach (var paymentInfoRowCarrier in paymentInfoCarrier.Rows)
                {
                    var currentPaymentInfoRowCarrier = currentPaymentInfoCarrier?.Rows?.FirstOrDefault(x => x.PaymentInfoRowID == paymentInfoRowCarrier.PaymentInfoRowID);
                    SetCarrierStateToUpdate(paymentInfoRowCarrier.CarrierState);
                    if (currentPaymentInfoRowCarrier != null)
                    {
                        paymentInfoRowCarrier.LastUpdatedDate = currentPaymentInfoRowCarrier.LastUpdatedDate;
                    }
                }
                foreach (var additionalPaymentInfoCarrier in paymentInfoCarrier.AdditionalPaymentInfo)
                {
                    var currentAdditionalPaymentInfoCarrier = currentPaymentInfoCarrier?.AdditionalPaymentInfo?.FirstOrDefault(x => x.ID == additionalPaymentInfoCarrier.ID);
                    SetCarrierStateToUpdate(additionalPaymentInfoCarrier.CarrierState);
                    if (currentAdditionalPaymentInfoCarrier != null)
                    {
                        additionalPaymentInfoCarrier.LastUpdatedDate = currentAdditionalPaymentInfoCarrier.LastUpdatedDate;
                    }
                }
                SetCarrierStateToUpdate(paymentInfoCarrier.BillingAddress.CarrierState);
                if (currentPaymentInfoCarrier?.BillingAddress != null)
                {
                    paymentInfoCarrier.BillingAddress.LastUpdatedDate = currentPaymentInfoCarrier.BillingAddress.LastUpdatedDate;
                }
            }
            foreach (var feeCarrier in orderCarrier.Fees)
            {
                var currentFeeCarrier = currentOrderCarrier.Fees?.FirstOrDefault(x => x.ID == feeCarrier.ID);
                SetCarrierStateToUpdate(feeCarrier.CarrierState);
                if (currentFeeCarrier != null)
                {
                    feeCarrier.LastUpdatedDate = currentFeeCarrier.LastUpdatedDate;
                }

            }
        }

        private void SetCarrierStateToUpdate(CarrierState carrierState)
        {
            carrierState.MarkAsNotModified();
            carrierState.IsMarkedForUpdating = true;
        }
    }
}
