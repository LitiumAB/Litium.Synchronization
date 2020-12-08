# Litium.Synchronization

## About

Litium.Synchronization is a tool to move Customers and Orders from one Litium installation to another without dependency on Litium version. 

This is useful during upgrades where the old installation can export all added information to the new environment as it is being added. At go live all traffic can be redirected to the new environment that already has all the data.

`Litium.Synchronization` is not a versioned addon, Litium does not maintain and upgrade the code for each new release of the platform. The code need to be tested before use in a production scenario.

### Version support

Litium.Synchronization.Export.L5 has references to Litium 5.6.8 and Litium.Synchronization.Import has references to Litium 7.4.1.

Adjust if you upgrade is between other Litium versions (only versions specified above are tested)

## Installation

1. Install nuget package references and compile the solution.

1. Add the following to appsettings of the web.config of the site that should be upgraded. 
    ```XML
    <add key=" SynchronizationExportFolder" value="Path to folder where exports will be added" />
    ```

1. Copy `Litium.Synchronization.Abstraction.dll` and `Litium.Synchronization.Export.L5.dll` to the bin folder of the site to upgrade. Do not copy the config files.

1. Clone your site upgrade the clone to litium the new Litium version.

1. Add the following to appsettings in web.config of your upgraded installation. 

    This configuration will add a prefix to orders and deliveries so that the orders that created in the upgraded version will not conflict with the orders that you are synchronizing.
    ```XML
    <add key="SynchronizationImportFolder" value="Path to folder where exports are added" />
    <add key="Litium:Sales:OrderNumberPrefix" value="Test" />
    <add key="Litium:Sales:DeliveryNumberPrefix" value="TestD" />
    ```

1. Add the following validator to the upgraded Accelerator project so that persons created in the upgraded version will not conflict with the orders that are synchronized.
    ```C#
    public class PersonIdValidator : ValidationRuleBase<Person>
    {
        public override ValidationResult Validate(Person entity, ValidationMode validationMode)
        {
            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                var id = Guid.NewGuid().ToString("N");
                entity.Id = $"Test{id}" ;
            }
            return new ValidationResult();
        }
    }
    ```

1. Add a similar validator for organziations
    ```C#
    public class OrganizationIdValidator : ValidationRuleBase<Organization>
    {
        public override ValidationResult Validate(Organization entity, ValidationMode validationMode)
        {
            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                var id = Guid.NewGuid().ToString("N");
                entity.Id = $"Test{id}" ;
            }
            return new ValidationResult();
        }
    }
    ```

1. Add the following ScheduledJobs to web.config of the upgraded solution
    ```XML
    <scheduledTask type="Litium.Synchronization.Import.Tasks.OrderImport, Litium.Synchronization.Import" startTime="00:00" interval="1m" parameters="MaxArchiveAgeInDays=7,MaxErrorAgeInMonths=1" />
    <scheduledTask type="Litium.Synchronization.Import.Tasks.OrganizationImport, Litium.Synchronization.Import" startTime="00:00" interval="1m" parameters="MaxArchiveAgeInDays=7,MaxErrorAgeInMonths=1" />
    <scheduledTask type="Litium.Synchronization.Import.Tasks.PersonImport, Litium.Synchronization.Import" startTime="00:00" interval="1m" parameters="MaxArchiveAgeInDays=7,MaxErrorAgeInMonths=1" /> 
    ```
1. Copy `Litium.Synchronization.Abstraction.dll` and `Litium.Synchronization.Import.dll` to the upgraded solution

## Before go live

1. Ensure that all data has been imported to the new environment

1. Remove following appsettings before go live
    ```XML
    <add key="Litium:Sales:OrderNumberPrefix" value="Test" />
    <add key="Litium:Sales:DeliveryNumberPrefix" value="TestD" />
    ```

1. Remove the ValidationRules for persons and organizations from the solution
