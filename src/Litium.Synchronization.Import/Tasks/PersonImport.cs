using AutoMapper;
using Dapper;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Foundation.Data.MSSQL.Utilities;
using Litium.Globalization;
using Litium.Synchronization.Abstraction;
using Litium.Synchronization.Abstraction.Customers;
using Newtonsoft.Json;
using System.IO;

namespace Litium.Synchronization.Import.Tasks
{
    public class PersonImport : DefaultImport
    {
        private readonly PersonService _personService;

        public PersonImport(PersonService personService, FieldDefinitionService fieldDefinitionService, FieldTemplateService fieldTemplateService, LanguageService languageService, OrganizationService organizationService) : base("Person", fieldTemplateService, fieldDefinitionService, languageService, organizationService) 
        {
            _personService = personService;
        }
        
        protected override BaseItem DeserializeItem(TextReader tr, JsonSerializer serializer)
        {
            return serializer.Deserialize(tr, typeof(Abstraction.Customers.Person)) as Abstraction.Customers.Person;
        }

        protected override void ProcessItem(BaseItem item)
        {
            var personItem = item as Abstraction.Customers.Person;
            if (personItem != null)
            {
                var person = Mapper.Map(personItem, typeof(Abstraction.Customers.Person), typeof(Litium.Customers.Person)) as Litium.Customers.Person;
                person.LoginCredential.Username = personItem.LoginInfo.Id;
                if (person != null && _fieldTemplateService.Get<PersonFieldTemplate>(person.FieldTemplateSystemId) != null)
                {
                    var existingPerson = _personService.Get(person.SystemId);
                    if (existingPerson != null)
                    {
                        SaveLoginInfo(personItem.LoginInfo);
                        existingPerson = existingPerson.MakeWritableClone();
                        existingPerson.Addresses = person.Addresses;
                        existingPerson.Email = person.Email;
                        CopyFields(existingPerson.Fields, personItem);
                        existingPerson.FieldTemplateSystemId = person.FieldTemplateSystemId;
                        existingPerson.FirstName = person.FirstName;
                        existingPerson.GroupLinks = person.GroupLinks;
                        existingPerson.Id = person.Id;
                        existingPerson.LastName = person.LastName;
                        existingPerson.OrganizationLinks = person.OrganizationLinks;
                        existingPerson.Phone = person.Phone;
                        existingPerson.Title = person.Title;
                        existingPerson.LoginCredential.Username = person.LoginCredential.Username;
                        _personService.Update(existingPerson);
                    }
                    else
                    {
                        _personService.Create(person);
                        SaveLoginInfo(personItem.LoginInfo);
                    }
                }
                else
                {
                    if (person == null)
                    {
                        throw new System.Exception(string.Format(Messages.PersonMappingError, personItem.Id));
                    }
                    else
                    {
                        throw new System.Exception(string.Format(Messages.FieldTemplateMissing, person.FieldTemplateSystemId.ToString()));
                    }
                }
            }
        }

        private void SaveLoginInfo(LoginInfo loginInfo)
        {
            using (var cn = MSSQLUtilities.GetConnection(null))
            {
                using (var tr = cn.BeginTransaction())
                {
                    cn.Execute(
                        @"IF EXISTS (SELECT 1 FROM Security.PasswordLoginInfo WHERE SystemId = @SystemId) 
                        BEGIN
                            UPDATE Security.PasswordLoginInfo SET AccessFailedCount = @AccessFailedCount, Id = @Id,
                            LastLockedOutDate = @LastLockedOutDate, LockoutEndDate = @LockoutEndDate, LockoutEnabled = @LockoutEnabled,
                            PasswordDate = @PasswordDate, PasswordHash = @PasswordHash, SecurityStamp = @SecurityStamp, PasswordExpirationDate = @PasswordExpirationDate
                            WHERE  SystemId = @SystemId
                        END
                        ELSE
                        BEGIN
                            INSERT INTO Security.PasswordLoginInfo (AccessFailedCount, Id, LastLockedOutDate, LockoutEndDate, LockoutEnabled, PasswordDate, PasswordHash, SecurityStamp, SystemId, PasswordExpirationDate)
                            VALUES (@AccessFailedCount, @Id, @LastLockedOutDate, @LockoutEndDate, @LockoutEnabled, @PasswordDate, @PasswordHash, @SecurityStamp, @SystemId, @PasswordExpirationDate)
                        END",
                        new
                        {
                            loginInfo.AccessFailedCount,
                            loginInfo.Id,
                            loginInfo.LastLockedOutDate,
                            loginInfo.LockoutEnabled,
                            loginInfo.LockoutEndDate,
                            loginInfo.PasswordDate,
                            loginInfo.PasswordExpirationDate,
                            loginInfo.PasswordHash,
                            loginInfo.SecurityStamp,
                            loginInfo.SystemId
                        }, tr);
                    tr.Commit();
                }
            }
        }
    }
}
