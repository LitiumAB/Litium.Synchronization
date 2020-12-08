using AutoMapper;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Foundation.Modules.ECommerce.Plugins.Payments;
using Litium.Foundation.Modules.Relations.Carriers;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Synchronization.Abstraction.Customers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;

namespace Litium.Synchronization.Import.Mappings
{
    public class CustomersMapping : IAutoMapperConfiguration
    {
        private readonly FieldDefinitionService _fieldDefinitionService;
        private readonly LanguageService _languageService;
        private readonly PersonService _personService;
        private readonly GroupService _groupService;
        private readonly OrganizationService _organizationService;
        private readonly AddressTypeService _addressTypeService;
        private bool _isTest = false;
        
        public CustomersMapping(FieldDefinitionService fieldDefinitionService, LanguageService languageService, 
        PersonService personService, GroupService groupService, OrganizationService organizationService, AddressTypeService addressTypeService)
        {
            var isTest = ConfigurationManager.AppSettings["IsTest"];
            bool.TryParse(isTest, out _isTest);
            _fieldDefinitionService = fieldDefinitionService;
            _languageService = languageService;
            _personService = personService;
            _groupService = groupService;
            _organizationService = organizationService;
            _addressTypeService = addressTypeService;
        }
        public void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Litium.Synchronization.Abstraction.Customers.Address, Litium.Customers.Address>()
            .ForMember(d => d.AddressTypeSystemId, m => m.MapFrom(s => GetAddressTypeSystemId(s.AddressType)));
            cfg.CreateMap<Litium.Synchronization.Abstraction.Customers.Organization, Litium.Customers.Organization>()
            // Fields and parent organization id has to be mapped separately
            .ForMember(d => d.Fields, m => m.MapFrom(s => GetFields(s.Fields, s.ParentOrganizationSystemId, s.LegalRegistrationNumber)));
            cfg.CreateMap<Litium.Synchronization.Abstraction.Customers.Person, Litium.Customers.Person>()
             .ForMember(d => d.GroupLinks, m => m.MapFrom(s => GetGroupLinks(s.GroupLinkIds)))
             .ForMember(d => d.Fields, m => m.MapFrom(s => GetFields(s.Fields, null, null)))
             .ForMember(d => d.OrganizationLinks, m => m.MapFrom(s => GetOrganizationLinks(s)));
        }

        private Guid GetAddressTypeSystemId(Litium.Synchronization.Abstraction.Customers.AddressType addressType)
        {
            var addressTypeItem = _addressTypeService.Get(addressType.ToString());
            return addressTypeItem.SystemId;
        }
        private List<PersonToGroupLink> GetGroupLinks(List<Guid> groupSystemIds)
        {
            var result = new List<PersonToGroupLink>();
            foreach (var groupSystemId in groupSystemIds)
            {
                if (_groupService.Get<Group>(groupSystemId) != null)
                {
                    result.Add(new PersonToGroupLink(groupSystemId));
                }
            }
            return result;
        }

        private List<PersonToOrganizationLink> GetOrganizationLinks(Litium.Synchronization.Abstraction.Customers.Person person)
        {
            var result = new List<PersonToOrganizationLink>();
            foreach (var organizationSystemId in person.OrganizationLinkIds)
            {
                if (_organizationService.Get(organizationSystemId) != null)
                {
                    var organizationLink = new PersonToOrganizationLink(organizationSystemId);
                    if (person.AssignedRoles != null)
                    {
                        var assignedRoleIds = new HashSet<Guid>();
                        foreach (var assignedRole in person.AssignedRoles.Where(x => x.OrganizationSystemId == organizationSystemId))
                        {
                            if (!assignedRoleIds.Contains(assignedRole.RoleSystemId))
                            {
                                assignedRoleIds.Add(assignedRole.RoleSystemId);
                            }
                        }
                        organizationLink.RoleSystemIds = assignedRoleIds;
                    }
                    result.Add(organizationLink);
                }
            }
            return result;
        }

        
        private MultiCultureFieldContainer GetFields(List<Field> fields, Guid? parentOrganizationSystemId, string legalRegistrationNumber)
        {
            var result = new MultiCultureFieldContainer();
            foreach (var field in fields)
            {
                var fieldDefinition = _fieldDefinitionService.Get(field.FieldDefinitionSystemId);
                if (fieldDefinition != null)
                {
                    if (fieldDefinition.FieldType == field.FieldType.ToString())
                    {
                        switch (field.FieldType)
                        {
                            case FieldType.Boolean:
                                AddSingleValue(result, fieldDefinition.Id, field.BooleanValue, fieldDefinition.MultiCulture);
                                break;
                            case FieldType.Decimal:
                                AddSingleValue(result, fieldDefinition.Id, field.DecimalValue, fieldDefinition.MultiCulture);
                                break;
                            case FieldType.DateTime:
                                AddSingleValue(result, fieldDefinition.Id, field.IndexedTextValue, fieldDefinition.MultiCulture);
                                break;
                            case FieldType.LimitedText:
                                AddSingleValue(result, fieldDefinition.Id, field.IndexedTextValue, fieldDefinition.MultiCulture);
                                break;
                            case FieldType.TextOption:
                                AddArrayValue(result, fieldDefinition.Id, field.TextOptionValue.Select(x => (object)x).ToList(), fieldDefinition.MultiCulture);
                                break;
                        }
                    }
                    else if (field.FieldType == FieldType.LimitedText && (fieldDefinition.FieldType == "Text" || fieldDefinition.FieldType == "MultirowText"))
                    {
                        AddSingleValue(result, fieldDefinition.Id, field.IndexedTextValue, fieldDefinition.MultiCulture);
                    }
                }
            }
            if (parentOrganizationSystemId.HasValue)
            {
                var fieldDefinition = _fieldDefinitionService.Get<CustomerArea>("ParentOrganization");
                if (fieldDefinition != null)
                {
                    AddSingleValue(result, fieldDefinition.Id, parentOrganizationSystemId, fieldDefinition.MultiCulture);
                }
            }
            if (!string.IsNullOrWhiteSpace(legalRegistrationNumber))
            {
                var fieldDefinition = _fieldDefinitionService.Get<CustomerArea>("LegalRegistrationNumber");
                if (fieldDefinition != null)
                {
                    AddSingleValue(result, fieldDefinition.Id, legalRegistrationNumber, fieldDefinition.MultiCulture);
                }
            }
            return result;
        }

        private void AddSingleValue(MultiCultureFieldContainer fields, string fieldDefinitonId, object value, bool isMulticulture)
        {
            if (isMulticulture)
            {
                foreach (var language in _languageService.GetAll())
                {
                    fields.AddOrUpdateValue(fieldDefinitonId, language.CultureInfo.Name, value);
                }
            }
            else
            {
                fields.AddOrUpdateValue(fieldDefinitonId, value);
            }
        }

        private void AddArrayValue(MultiCultureFieldContainer fields, string fieldDefinitonId, List<object> values, bool isMulticulture)
        {
            if (isMulticulture)
            {
                var languages = _languageService.GetAll();
                foreach (var value in values)
                {
                    foreach (var language in languages)
                    {
                        fields.AddOrUpdateValue(fieldDefinitonId, language.CultureInfo.Name, value);
                    }
                }
            }
            else
            {
                foreach (var value in values)
                {
                    fields.AddOrUpdateValue(fieldDefinitonId, value);
                }
            }
        }
    }
}
