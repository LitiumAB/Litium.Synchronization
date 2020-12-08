using AutoMapper;
using Litium.Foundation.Modules.CMS;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using Litium.Synchronization.Abstraction.Sales;
using System;
using System.Configuration;

namespace Litium.Synchronization.Import.Mappings
{
    public class SalesMapping : IAutoMapperConfiguration
    {
         public void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<AdditionalDeliveryInfo, AdditionalDeliveryInfoCarrier>()
                .ForMember(d => d.DeliveryID, m => m.MapFrom(s => s.DeliverySystemId))
                .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
                .ForMember(d => d.ID, m => m.MapFrom(s => s.SystemId));
            cfg.CreateMap<AdditionalOrderInfo, AdditionalOrderInfoCarrier>()
                .ForMember(d => d.OrderID, m => m.MapFrom(s => s.OrderSystemId))
                .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
                .ForMember(d => d.ID, m => m.MapFrom(s => s.SystemId));
            cfg.CreateMap<AdditionalPaymentInfo, AdditionalPaymentInfoCarrier>()
                .ForMember(d => d.PaymentInfoID, m => m.MapFrom(s => s.PaymentInfoSystemId))
                .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
                .ForMember(d => d.ID, m => m.MapFrom(s => s.SystemId));
            cfg.CreateMap<Address, AddressCarrier>()
                .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
                .ForMember(d => d.ID, m => m.MapFrom(s => s.SystemId));
            cfg.CreateMap<CampaignConditionInfo, CampaignConditionInfoCarrier>()
               .ForMember(d => d.CampaignID, m => m.MapFrom(s => s.CampaignSystemId))
               .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
               .ForMember(d => d.ConditionInfoID, m => m.MapFrom(s => s.SystemId));
            cfg.CreateMap<CustomerInfo, CustomerInfoCarrier>()
               .ForMember(d => d.AddressID, m => m.MapFrom(s => s.AddressSystemId))
               .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
               .ForMember(d => d.OrganizationID, m => m.MapFrom(s => s.OrganizationSystemId))
               .ForMember(d => d.PersonID, m => m.MapFrom(s => s.PersonSystemId))
               .ForMember(d => d.ID, m => m.MapFrom(s => s.SystemId));
            cfg.CreateMap<Delivery, DeliveryCarrier>()
               .ForMember(d => d.CampaignID, m => m.MapFrom(s => s.CampaignSystemId))
               .ForMember(d => d.DeliveryMethodID, m => m.MapFrom(s => s.DeliveryMethodSystemId))
               .ForMember(d => d.DeliveryProviderID, m => m.MapFrom(s => s.DeliveryProviderSystemId))
               .ForMember(d => d.ExternalCampaignID, m => m.MapFrom(s => s.ExternalCampaignId))
               .ForMember(d => d.ExternalReferenceID, m => m.MapFrom(s => s.ExternalReferenceId))
               .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
               .ForMember(d => d.OrderID, m => m.MapFrom(s => s.OrderSystemId))
               .ForMember(d => d.ID, m => m.MapFrom(s => s.SystemId));
            cfg.CreateMap<Fee, FeeCarrier>()
              .ForMember(d => d.CampaignID, m => m.MapFrom(s => s.CampaignSystemId))
              .ForMember(d => d.ExternalCampaignID, m => m.MapFrom(s => s.ExternalCampaignId))
              .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
              .ForMember(d => d.OrderID, m => m.MapFrom(s => s.OrderSystemId))
              .ForMember(d => d.ID, m => m.MapFrom(s => s.SystemId));
            cfg.CreateMap<Order, OrderCarrier>()
            .ForMember(d => d.ChannelID, m => m.MapFrom(s => s.ChannelSystemId))
            .ForMember(d => d.CountryID, m => m.MapFrom(s => s.CountrySystemId))
            .ForMember(d => d.CurrencyID, m => m.MapFrom(s => s.CurrencySystemId))
            .ForMember(d => d.ExternalOrderID, m => m.MapFrom(s => s.ExternalOrderId))
            .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
            .ForMember(d => d.MarketID, m => m.MapFrom(s => s.MarketSystemId))
            .ForMember(d => d.ID, m => m.MapFrom(s => s.SystemId))
            .ForMember(d => d.WebSiteID, m => m.MapFrom(s => s.WebsiteSystemId));
            cfg.CreateMap<OrderDiscount, OrderDiscountCarrier>()
              .ForMember(d => d.CampaignID, m => m.MapFrom(s => s.CampaignSystemId))
              .ForMember(d => d.ExternalCampaignID, m => m.MapFrom(s => s.ExternalCampaignId))
              .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
              .ForMember(d => d.OrderID, m => m.MapFrom(s => s.OrderSystemId))
              .ForMember(d => d.ID, m => m.MapFrom(s => s.SystemId));
            cfg.CreateMap<OrderRow, OrderRowCarrier>()
              .ForMember(d => d.CampaignID, m => m.MapFrom(s => s.CampaignSystemId))
              .ForMember(d => d.DeliveryID, m => m.MapFrom(s => s.DeliverySystemId))
              .ForMember(d => d.ExternalCampaignID, m => m.MapFrom(s => s.ExternalCampaignId))
              .ForMember(d => d.ExternalOrderRowID, m => m.MapFrom(s => s.ExternalOrderRowId))
              .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
              .ForMember(d => d.OrderID, m => m.MapFrom(s => s.OrderSystemId))
              .ForMember(d => d.ProductID, m => m.MapFrom(s => s.ProductSystemId))
              .ForMember(d => d.ID, m => m.MapFrom(s => s.SystemId));
            cfg.CreateMap<PaymentInfo, PaymentInfoCarrier>()
              .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
              .ForMember(d => d.OrderID, m => m.MapFrom(s => s.OrderSystemId))
              .ForMember(d => d.ReferenceID, m => m.MapFrom(s => s.ReferenceId))
              .ForMember(d => d.ID, m => m.MapFrom(s => s.SystemId));
            cfg.CreateMap<PaymentInfoRow, PaymentInfoRowCarrier>()
             .ForMember(d => d.LastUpdatedUserID, m => m.MapFrom(s => s.LastUpdatedUserSystemId))
             .ForMember(d => d.PaymentInfoID, m => m.MapFrom(s => s.PaymentInfoSystemId))
             .ForMember(d => d.ReferenceID, m => m.MapFrom(s => s.ReferenceId))
             .ForMember(d => d.ReferenceType, m => m.MapFrom(s => (Foundation.Modules.ECommerce.Payments.PaymentInfoRowType) s.ReferenceType))
             .ForMember(d => d.PaymentInfoRowID, m => m.MapFrom(s => s.SystemId));
        }
    }
}
