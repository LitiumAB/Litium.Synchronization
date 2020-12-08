using AutoMapper;
using Litium.Foundation.Modules.CMS;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using Litium.Synchronization.Abstraction.Sales;
using System;
using System.Configuration;

namespace Litium.Synchronization.Export.L5.Mappings
{
    public class SalesMapping : IAutoMapperConfiguration
    {
        public const string TestWebsiteCulture = "sv-SE";
        public const string TestWebsiteDomainName = "Test website";
        public const string TestWebsiteName = "Test website";

        private bool _isTest = false;
        public SalesMapping()
        {
            var isTest = ConfigurationManager.AppSettings["IsSysnchronizationTest"];
            bool.TryParse(isTest, out _isTest);
        }
        public void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<AdditionalDeliveryInfoCarrier, AdditionalDeliveryInfo>()
                .ForMember(d => d.DeliverySystemId, m => m.MapFrom(s => s.DeliveryID))
                .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
                .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID));
            cfg.CreateMap<AdditionalOrderInfoCarrier, AdditionalOrderInfo>()
                .ForMember(d => d.OrderSystemId, m => m.MapFrom(s => s.OrderID))
                .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
                .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID));
            cfg.CreateMap<AdditionalPaymentInfoCarrier, AdditionalPaymentInfo>()
                .ForMember(d => d.PaymentInfoSystemId, m => m.MapFrom(s => s.PaymentInfoID))
                .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
                .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID));
            cfg.CreateMap<AddressCarrier, Address>()
                .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
                .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID));
            cfg.CreateMap<CampaignConditionInfoCarrier, CampaignConditionInfo>()
               .ForMember(d => d.CampaignSystemId, m => m.MapFrom(s => s.CampaignID))
               .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
               .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ConditionInfoID));
            cfg.CreateMap<CustomerInfoCarrier, CustomerInfo>()
               .ForMember(d => d.AddressSystemId, m => m.MapFrom(s => s.AddressID))
               .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
               .ForMember(d => d.OrganizationSystemId, m => m.MapFrom(s => s.OrganizationID))
               .ForMember(d => d.PersonSystemId, m => m.MapFrom(s => s.PersonID))
               .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID));
            cfg.CreateMap<DeliveryCarrier, Delivery>()
               .ForMember(d => d.CampaignSystemId, m => m.MapFrom(s => s.CampaignID))
               .ForMember(d => d.DeliveryMethodSystemId, m => m.MapFrom(s => s.DeliveryMethodID))
               .ForMember(d => d.DeliveryProviderSystemId, m => m.MapFrom(s => s.DeliveryProviderID))
               .ForMember(d => d.ExternalCampaignId, m => m.MapFrom(s => s.ExternalCampaignID))
               .ForMember(d => d.ExternalReferenceId, m => m.MapFrom(s => s.ExternalReferenceID))
               .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
               .ForMember(d => d.OrderSystemId, m => m.MapFrom(s => s.OrderID))
               .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID));
            cfg.CreateMap<FeeCarrier, Fee>()
              .ForMember(d => d.CampaignSystemId, m => m.MapFrom(s => s.CampaignID))
              .ForMember(d => d.ExternalCampaignId, m => m.MapFrom(s => s.ExternalCampaignID))
              .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
              .ForMember(d => d.OrderSystemId, m => m.MapFrom(s => s.OrderID))
              .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID));
            cfg.CreateMap<OrderCarrier, Order>()
            .ForMember(d => d.CurrencySystemId, m => m.MapFrom(s => s.CurrencyID))
            .ForMember(d => d.ExternalOrderId, m => m.MapFrom(s => s.ExternalOrderID))
            .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
            .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID))
            .ForMember(d => d.Website, m => m.MapFrom(s => GetWebsite(s.WebSiteID, s.LastUpdatedUserID, s.LastUpdatedDate)))
            .ForMember(d => d.WebsiteSystemId, m => m.MapFrom(s => s.WebSiteID));
            cfg.CreateMap<OrderDiscountCarrier, OrderDiscount>()
              .ForMember(d => d.CampaignSystemId, m => m.MapFrom(s => s.CampaignID))
              .ForMember(d => d.ExternalCampaignId, m => m.MapFrom(s => s.ExternalCampaignID))
              .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
              .ForMember(d => d.OrderSystemId, m => m.MapFrom(s => s.OrderID))
              .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID));
            cfg.CreateMap<OrderRowCarrier, OrderRow>()
              .ForMember(d => d.CampaignSystemId, m => m.MapFrom(s => s.CampaignID))
              .ForMember(d => d.DeliverySystemId, m => m.MapFrom(s => s.DeliveryID))
              .ForMember(d => d.ExternalCampaignId, m => m.MapFrom(s => s.ExternalCampaignID))
              .ForMember(d => d.ExternalOrderRowId, m => m.MapFrom(s => s.ExternalOrderRowID))
              .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
              .ForMember(d => d.OrderSystemId, m => m.MapFrom(s => s.OrderID))
              .ForMember(d => d.ProductSystemId, m => m.MapFrom(s => s.ProductID))
              .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID));
            cfg.CreateMap<PaymentInfoCarrier, PaymentInfo>()
              .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
              .ForMember(d => d.OrderSystemId, m => m.MapFrom(s => s.OrderID))
              .ForMember(d => d.ReferenceId, m => m.MapFrom(s => s.ReferenceID))
              .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID));
            cfg.CreateMap<PaymentInfoRowCarrier, PaymentInfoRow>()
             .ForMember(d => d.LastUpdatedUserSystemId, m => m.MapFrom(s => s.LastUpdatedUserID))
             .ForMember(d => d.PaymentInfoSystemId, m => m.MapFrom(s => s.PaymentInfoID))
             .ForMember(d => d.ReferenceId, m => m.MapFrom(s => s.ReferenceID))
             .ForMember(d => d.ReferenceType, m => m.MapFrom(s => (byte)s.ReferenceType))
             .ForMember(d => d.SystemId, m => m.MapFrom(s => s.PaymentInfoRowID));
        }

        private Website GetWebsite(Guid websiteId, Guid lastUpdatedUserSysteId, DateTime lastUpdatedDate)
        {
            if (_isTest)
            {
                return new Website() { Culture = TestWebsiteCulture, SystemId = websiteId, Name = TestWebsiteName, DomainName = TestWebsiteDomainName };
            }
            var website = ModuleCMS.Instance.WebSites.GetWebSite(websiteId);
            return website != null ? 
            new Website() {
                Culture = website.Culture.Name,
                DomainName = website.DomainName,
                LastUpdatedDate = lastUpdatedDate,
                LastUpdatedUserSystemId = lastUpdatedUserSysteId,
                Name = website.Name,
                SystemId = website.ID
            }
            : null;
        }
    }
}
