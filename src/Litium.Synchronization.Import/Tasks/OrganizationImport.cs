using AutoMapper;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Synchronization.Abstraction;
using Newtonsoft.Json;
using System.IO;

namespace Litium.Synchronization.Import.Tasks
{
    public class OrganizationImport : DefaultImport
    {
        public OrganizationImport(OrganizationService organizationService, FieldDefinitionService fieldDefinitionService, FieldTemplateService fieldTemplateService, LanguageService languageService) : base("Organization", fieldTemplateService, fieldDefinitionService, languageService, organizationService) { }
        
        protected override BaseItem DeserializeItem(TextReader tr, JsonSerializer serializer)
        {
            return serializer.Deserialize(tr, typeof(Abstraction.Customers.Organization)) as Abstraction.Customers.Organization;
        }

        protected override void ProcessItem(BaseItem item)
        {
            var organizationItem = item as Abstraction.Customers.Organization;
            if (organizationItem != null)
            {
                var organization = Mapper.Map(organizationItem, typeof(Abstraction.Customers.Organization), typeof(Litium.Customers.Organization)) as Litium.Customers.Organization;
                if (organization != null && _fieldTemplateService.Get<OrganizationFieldTemplate>(organization.FieldTemplateSystemId) != null)
                {
                    var existingOrganization = _organizationService.Get(organization.SystemId);
                    if (existingOrganization != null)
                    {
                        existingOrganization = existingOrganization.MakeWritableClone();
                        existingOrganization.Addresses = organization.Addresses;
                        CopyFields(existingOrganization.Fields, organizationItem);
                        existingOrganization.FieldTemplateSystemId = organization.FieldTemplateSystemId;
                        existingOrganization.Id = organization.Id;
                        existingOrganization.Name = organization.Name;
                        _organizationService.Update(existingOrganization);
                    }
                    else
                    {
                        _organizationService.Create(organization);
                    }
                }
                else
                {
                    if (organization == null)
                    {
                        throw new System.Exception(string.Format(Messages.OrganizationMappingError, organizationItem.Id));
                    }
                    else
                    {
                        throw new System.Exception(string.Format(Messages.FieldTemplateMissing, organization.FieldTemplateSystemId.ToString()));
                    }
                }
            }
        }
    }
}
