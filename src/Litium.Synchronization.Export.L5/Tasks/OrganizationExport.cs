using AutoMapper;
using Litium.Foundation.Modules.Relations;
using Litium.Foundation.Modules.Relations.Carriers;
using Litium.Owin.Lifecycle;
using Litium.Runtime;
using Litium.Synchronization.Abstraction;
using Litium.Synchronization.Abstraction.Customers;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Litium.Synchronization.Export.L5.Tasks
{
    [Autostart]
    public class OrganizationExport : DefaultExport
    {
        public OrganizationExport() : base("Organization")
        {
            var eventManager = ModuleRelations.Instance.EventManager;
            eventManager.OrganizationCreated += OrganizationUpdated;
            eventManager.OrganizationUpdated += OrganizationUpdated;
        }

        private void OrganizationUpdated(Guid organizationSystemId, Guid parentOrganizationSystemId)
        {
            var organization = ModuleRelations.Instance.Organizations.GetOrganization(organizationSystemId);
            var organizationCarrier = organization?.GetAsCarrier(true, true);
            if (organizationCarrier != null)
            {
                try
                {
                    var item = Mapper.Map(organizationCarrier, typeof(OrganizationCarrier), typeof(Organization)) as Organization;
                    if (item != null)
                    {
                        WriteFile(item);
                    }
                    else
                    {
                        throw new Exception(string.Format(Messages.OrganizationMappingError, organizationCarrier.CustomerNumber));
                    }
                }
                catch (Exception exc)
                {
                    _log.Error(string.Format(Messages.ExportingOrganizationError, organizationCarrier.CustomerNumber), exc);
                }
            }
            else
            {
                _log.Error(string.Format(Messages.OrganizationIsMissing, organizationSystemId.ToString()));
            }
        }

        protected override string GetFileName(BaseItem item)
        {
            var organization = item as Organization;
            return organization != null ? $"{organization.Id}-{DateTime.Now.Ticks}-{Guid.NewGuid()}" : string.Empty;
        }

        protected override void SerializeItem(TextWriter textWriter, JsonSerializer serializer, BaseItem item)
        {
            serializer.Serialize(textWriter, item as Organization);
        }
    }
}
