using AutoMapper;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Runtime;
using Litium.Synchronization.Abstraction;
using Litium.Synchronization.Abstraction.Sales;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Litium.Synchronization.Export.L5.Tasks
{
    [Autostart]
    public class OrderExport : DefaultExport
     {
        public OrderExport() : base("Order") 
        {
            var eventManager = ModuleECommerce.Instance.EventManager;
            eventManager.OrderCreated += OrderUpdated;
            eventManager.OrderUpdated += OrderUpdated;
        }
        
        private void OrderUpdated(Guid orderSystemId)
        {
            var order = ModuleECommerce.Instance.Orders.GetOrder(orderSystemId, ModuleECommerce.Instance.AdminToken);
            var orderCarrier = order?.GetAsCarrier(true, true, true, true, true, true);
            if (orderCarrier != null)
            {
                try
                {
                    var item = Mapper.Map(orderCarrier, typeof(OrderCarrier), typeof(Order)) as Order;
                    if (item != null)
                    {
                        WriteFile(item);
                    }
                    else
                    {
                        throw new Exception(string.Format(Messages.OrderMappingError, orderCarrier.ExternalOrderID));
                    }
                }
                catch (Exception exc)
                {
                    _log.Error(string.Format(Messages.ExportingOrderError, orderCarrier.ExternalOrderID), exc);
                }
            }
            else
            {
                _log.Error(string.Format(Messages.OrderIsMissing, orderSystemId.ToString()));
            }
        }

        protected override string GetFileName(BaseItem item)
        {
            var order = item as Order;
            return order != null ? $"{order.ExternalOrderId}-{DateTime.Now.Ticks}-{Guid.NewGuid()}" : string.Empty;
        }

        protected override void SerializeItem(TextWriter textWriter, JsonSerializer serializer, BaseItem item)
        {
            serializer.Serialize(textWriter, item as Order);
        }
    }
}
