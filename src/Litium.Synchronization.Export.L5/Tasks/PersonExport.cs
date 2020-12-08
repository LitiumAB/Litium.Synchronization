using AutoMapper;
using Dapper;
using Litium.Foundation;
using Litium.Foundation.Data.MSSQL.Utilities;
using Litium.Foundation.Modules.Relations;
using Litium.Foundation.Modules.Relations.Carriers;
using Litium.Owin.Lifecycle;
using Litium.Runtime;
using Litium.Synchronization.Abstraction;
using Litium.Synchronization.Abstraction.Customers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Litium.Synchronization.Export.L5.Tasks
{
    [Autostart]
    public class PersonExport : DefaultExport
    {
        public PersonExport() : base("Person")
        {
            var eventManager = ModuleRelations.Instance.EventManager;
            eventManager.PersonCreated += PersonUpdated;
            eventManager.PersonUpdated += PersonUpdated;
            Solution.Instance.EventManager.UserUpdated += PersonUpdated;
        }

        private void PersonUpdated(Guid personSystemId)
        {
            var person = ModuleRelations.Instance.Persons.GetPerson(personSystemId);
            var personCarrier = person?.GetAsCarrier(false, true, true, true);
            if (personCarrier != null)
            {
                try
                {
                    var item = Mapper.Map(personCarrier, typeof(PersonCarrier), typeof(Person)) as Person;
                    // Login information is not mapped by the mapper has to be gathered explicitly
                    if (item != null)
                    {
                        item.LoginInfo = GetLoginInfo(item.SystemId, personCarrier.AccountIsLockedOut);
                        WriteFile(item);
                    }
                    else
                    {
                        throw new Exception(string.Format(Messages.PersonMappingError, personCarrier.CustomerNumber));
                    }
                }
                catch (Exception exc)
                {
                    _log.Error(string.Format(Messages.ExportingPersonError, personCarrier.CustomerNumber), exc);
                }
            }
            else
            {
                _log.Error(string.Format(Messages.PersonIsMissing, personSystemId.ToString()));
            }
        }

        protected override string GetFileName(BaseItem item)
        {
            var person = item as Person;
            return person != null ? $"{person.Id}-{DateTime.Now.Ticks}" : string.Empty;
        }

        protected override void SerializeItem(TextWriter textWriter, JsonSerializer serializer, BaseItem item)
        {
            serializer.Serialize(textWriter, item as Person);
        }

        private LoginInfo GetLoginInfo(Guid personSystemId, bool? accountIsLockedOut)
        {
            var lockoutEnabled = accountIsLockedOut.HasValue ? accountIsLockedOut.Value : false;
            using (var cn = MSSQLUtilities.GetConnection(null))
            {
                using (var tr = cn.BeginTransaction())
                {
                    var result = cn.Query<LoginInfo>(
                        @"SELECT LoginFailureCount AS AccessFailedCount, LoginName AS Id, LastLockedOutDateTime AS LastLockedOutDate, LockoutEndDateUtc AS LockoutEndDate,
                        @lockoutEnabled AS LockoutEnabled, PasswordDate, Password as PasswordHash, SecurityStamp, @personSystemId AS SystemId, NULL as PasswordExpirationDate
                        FROM Foundation_User WHERE UserId = @personSystemId",
                        new
                        {
                            lockoutEnabled,
                            personSystemId,
                        }, tr).FirstOrDefault();
                    tr.Commit();
                    return result;
                }
            }
        }
    }
}
