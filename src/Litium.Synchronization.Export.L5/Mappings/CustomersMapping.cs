using AutoMapper;
using Litium.Foundation.Modules.Relations.Carriers;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using Litium.Synchronization.Abstraction.Customers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Litium.Synchronization.Export.L5.Mappings
{
    public class CustomersMapping : IAutoMapperConfiguration
    {
        private bool _isTest = false;
        public const int TestAccessFailedCount = 1;
        public const string TestPasswordHash = "Password hash";
        public const string TestSecurityStamp = "Security stamp";
        public DateTime TestPasswordDate { get; private set; }
        public DateTime TestLastLockedOutDate
        {
            get { return TestPasswordDate.AddDays(-5); }
        }
        public DateTime TestLockoutEndDate
        {
            get { return TestPasswordDate.AddDays(5); }
        }
        public DateTime TestPasswordExpirationDate
        {
            get { return TestPasswordDate.AddDays( 120); }
        }
        public CustomersMapping()
        {
            var isTest = ConfigurationManager.AppSettings["IsSysnchronizationTest"];
            bool.TryParse(isTest, out _isTest);
            TestPasswordDate = DateTime.Now;
        }
        public void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<AddressCarrier, Address>()
               .ForMember(d => d.SystemId, m => m.MapFrom(s => s.AddressID))
               .ForMember(d => d.PhoneNumber, m => m.MapFrom(s => s.Phone))
               .ForMember(d => d.ZipCode, m => m.MapFrom(s => s.Zip));
            cfg.CreateMap<AssignedRoleCarrier, AssignedRole>()
               .ForMember(d => d.OrganizationSystemId, m => m.MapFrom(s => s.Organization))
               .ForMember(d => d.PersonSystemId, m => m.MapFrom(s => s.Person))
               .ForMember(d => d.RoleSystemId, m => m.MapFrom(s => s.Role));
            cfg.CreateMap<BooleanFieldCarrier, Field>()
               .ForMember(d => d.BooleanValue, m => m.MapFrom(s => s.Value))
               .ForMember(d => d.EntityType, m => m.MapFrom(s => EntityType.None))
               .ForMember(d => d.FieldDefinitionSystemId, m => m.MapFrom(s => s.FieldDefinitionID))
               .ForMember(d => d.FieldType, m => m.MapFrom(s => FieldType.Boolean))
               .ForMember(d => d.Index, m => m.MapFrom(s => 0))
               .ForMember(d => d.IsArray, m => m.MapFrom(s => false))
               .ForMember(d => d.IsMultiSelect, m => m.MapFrom(s => false))
               .ForMember(d => d.OwnerSystemId, m => m.MapFrom(s => GetOwnerSystemId(s)));
            cfg.CreateMap<DateTimeFieldCarrier, Field>()
               .ForMember(d => d.DateTimeValue, m => m.MapFrom(s => s.Value))
               .ForMember(d => d.EntityType, m => m.MapFrom(s => EntityType.None))
               .ForMember(d => d.FieldDefinitionSystemId, m => m.MapFrom(s => s.FieldDefinitionID))
               .ForMember(d => d.FieldType, m => m.MapFrom(s => FieldType.DateTime))
               .ForMember(d => d.Index, m => m.MapFrom(s => 0))
               .ForMember(d => d.IsArray, m => m.MapFrom(s => false))
               .ForMember(d => d.IsMultiSelect, m => m.MapFrom(s => false))
               .ForMember(d => d.OwnerSystemId, m => m.MapFrom(s => GetOwnerSystemId(s)));
            cfg.CreateMap<ImageFieldCarrier, Field>()
               .ForMember(d => d.EntityType, m => m.MapFrom(s => EntityType.None))
               .ForMember(d => d.FieldDefinitionSystemId, m => m.MapFrom(s => s.FieldDefinitionID))
               .ForMember(d => d.FieldType, m => m.MapFrom(s => FieldType.MediaPointerImage))
               .ForMember(d => d.Index, m => m.MapFrom(s => 0))
               .ForMember(d => d.IsMultiSelect, m => m.MapFrom(s => false))
               .ForMember(d => d.OwnerSystemId, m => m.MapFrom(s => GetOwnerSystemId(s)));
            cfg.CreateMap<NumberFieldCarrier, Field>()
               .ForMember(d => d.DecimalValue, m => m.MapFrom(s => Convert.ToDecimal(s.Value)))
               .ForMember(d => d.EntityType, m => m.MapFrom(s => EntityType.None))
               .ForMember(d => d.FieldDefinitionSystemId, m => m.MapFrom(s => s.FieldDefinitionID))
               .ForMember(d => d.FieldType, m => m.MapFrom(s => FieldType.Decimal))
               .ForMember(d => d.Index, m => m.MapFrom(s => 0))
               .ForMember(d => d.IsArray, m => m.MapFrom(s => false))
               .ForMember(d => d.IsMultiSelect, m => m.MapFrom(s => false))
               .ForMember(d => d.OwnerSystemId, m => m.MapFrom(s => GetOwnerSystemId(s)));
            cfg.CreateMap<MultiSelectFieldCarrier, Field>()
               .ForMember(d => d.EntityType, m => m.MapFrom(s => EntityType.None))
               .ForMember(d => d.FieldDefinitionSystemId, m => m.MapFrom(s => s.FieldDefinitionID))
               .ForMember(d => d.FieldType, m => m.MapFrom(s => FieldType.TextOption))
               .ForMember(d => d.Index, m => m.MapFrom(s => 0))
               .ForMember(d => d.IsArray, m => m.MapFrom(s => false))
               .ForMember(d => d.IsMultiSelect, m => m.MapFrom(s => true))
               .ForMember(d => d.OwnerSystemId, m => m.MapFrom(s => GetOwnerSystemId(s)))
               .ForMember(d => d.TextOptionValue, m => m.MapFrom(s => s.Values.ToList()));
            cfg.CreateMap<StringFieldCarrier, Field>()
                .ForMember(d => d.EntityType, m => m.MapFrom(s => EntityType.None))
                .ForMember(d => d.FieldDefinitionSystemId, m => m.MapFrom(s => s.FieldDefinitionID))
                .ForMember(d => d.FieldType, m => m.MapFrom(s => FieldType.LimitedText))
                .ForMember(d => d.Index, m => m.MapFrom(s => 0))
                .ForMember(d => d.IndexedTextValue, m => m.MapFrom(s => s.Value))
                .ForMember(d => d.IsArray, m => m.MapFrom(s => false))
                .ForMember(d => d.IsMultiSelect, m => m.MapFrom(s => false))
                .ForMember(d => d.OwnerSystemId, m => m.MapFrom(s => GetOwnerSystemId(s)));
            cfg.CreateMap<OrganizationCarrier, Organization>()
               .ForMember(d => d.Addresses, m => m.MapFrom(s => GetAddresses(s.Addresses)))
               .ForMember(d => d.FieldTemplateSystemId, m => m.MapFrom(s => s.FieldTemplateID))
               .ForMember(d => d.Fields, m => m.MapFrom(s => s.CustomFields.ToList()))
               .ForMember(d => d.Id, m => m.MapFrom(s => s.CustomerNumber))
               .ForMember(d => d.ParentOrganizationSystemId, m => m.MapFrom(s => s.ParentOrganizationID))
               .ForMember(d => d.PersonLinkIds, m => m.MapFrom(s => s.Members))
               .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID));
            cfg.CreateMap<PersonCarrier, Person>()
             .ForMember(d => d.Addresses, m => m.MapFrom(s => GetAddresses(s)))
             .ForMember(d => d.FieldTemplateSystemId, m => m.MapFrom(s => s.FieldTemplateID))
             .ForMember(d => d.GroupLinkIds, m => m.MapFrom(s => s.Groups.ToList()))
             .ForMember(d => d.Fields, m => m.MapFrom(s => s.CustomFields.ToList()))
             .ForMember(d => d.Id, m => m.MapFrom(s => s.CustomerNumber))
             .ForMember(d => d.LoginInfo, m => m.MapFrom(s => GetLoginInfo(s)))
             .ForMember(d => d.OrganizationLinkIds, m => m.MapFrom(s => s.Organizations.ToList()))
             .ForMember(d => d.SystemId, m => m.MapFrom(s => s.ID));
        }

        private List<Address> GetAddresses(AddressCarrier[] addressCarriers)
        {
            var result = new List<Address>();
            foreach(var addressCarrier in addressCarriers)
            { 
                var address = Mapper.Map(addressCarrier, typeof(AddressCarrier), typeof(Address)) as Address;
                if (address != null)
                {
                    switch (addressCarrier.AddressType)
                    {
                        case Foundation.Modules.Relations.Addresses.AddressType.Billing:
                            address.AddressType = AddressType.Billing;
                            break;
                        case Foundation.Modules.Relations.Addresses.AddressType.Delivery:
                            address.AddressType = AddressType.Delivery;
                            break;
                        default:
                            address.AddressType = AddressType.BillingAndDelivery;
                            break;
                    }
                    result.Add(address);
                }
            }
            return result;
        }

        private List<Address> GetAddresses(PersonCarrier personCarrier)
        {
            var result = new List<Address>();
            if (personCarrier.Address != null)
            {
                var address = Mapper.Map(personCarrier.Address, typeof(AddressCarrier), typeof(Address)) as Address;
                if (address != null)
                {
                    address.AddressType = AddressType.Address;
                    result.Add(address);
                }
            }
            if (personCarrier.DeliveryAddress != null)
            {
                var address = Mapper.Map(personCarrier.DeliveryAddress, typeof(AddressCarrier), typeof(Address)) as Address;
                if (address != null)
                {
                    address.AddressType = AddressType.AlternativeAddress;
                    result.Add(address);
                }
            }
            return result;
        }

        private Guid GetOwnerSystemId(FieldCarrier fieldCarrier)
        {
            return fieldCarrier.PersonID != Guid.Empty
            ? fieldCarrier.PersonID
            : (fieldCarrier.OrganizationID != Guid.Empty ? fieldCarrier.OrganizationID : fieldCarrier.GroupID);
        }

        private LoginInfo GetLoginInfo(PersonCarrier personCarrier)
        {
            if (_isTest)
            {
                return new LoginInfo()
                {
                    LockoutEnabled = true,
                    AccessFailedCount = TestAccessFailedCount,
                    Id = personCarrier.Login,
                    SystemId = personCarrier.ID,
                    LastLockedOutDate = TestLastLockedOutDate,
                    LockoutEndDate = TestLockoutEndDate,
                    PasswordDate = TestPasswordDate,
                    PasswordExpirationDate = TestPasswordExpirationDate,
                    PasswordHash = TestPasswordHash,
                    SecurityStamp = TestSecurityStamp
                };
            }
            return new LoginInfo();
        }
    }
}
