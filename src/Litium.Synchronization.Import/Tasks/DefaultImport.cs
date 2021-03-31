using Litium.Common;
using Litium.ComponentModel;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Foundation.Configuration;
using Litium.Foundation.Languages;
using Litium.Foundation.Security;
using Litium.Foundation.Tasks;
using Litium.Globalization;
using Litium.Owin.Logging;
using Litium.Synchronization.Abstraction;
using Litium.Synchronization.Abstraction.Customers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Litium.Synchronization.Import.Tasks
{
    public abstract class DefaultImport: ITask, IDisposable
    {
        private const string _archive = "Archive";
        private const string _error = "Error";
        private static readonly object _lock = new object();
        private const int MaxRetryCount = 100; // 100ms x 20 times = 2 sec wait time for each file.
        private const int MaxWaitTime = 20; //20 ms.
        protected readonly string _path;
        protected readonly ILog _log;
        protected readonly FieldTemplateService _fieldTemplateService;
        protected readonly FieldDefinitionService _fieldDefinitionService;
        private readonly LanguageService _languageService;
        protected readonly OrganizationService _organizationService;
        private bool _isExecuting = false;

        protected DefaultImport(string directory)
        {
            _log = GetType().Log();
            _path = Path.Combine(ConfigurationManager.AppSettings["SynchronizationImportFolder"].NullIfEmpty() ?? Path.Combine(GeneralConfig.Instance.CommonFilesDirectory, "SynchronizationExport"), directory);
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }
        protected DefaultImport(string directory, FieldTemplateService fieldTemplateService, FieldDefinitionService fieldDefinitionService, LanguageService languageService, OrganizationService organizationService):this(directory)
        {
            _fieldTemplateService = fieldTemplateService;
            _fieldDefinitionService = fieldDefinitionService;
            _languageService = languageService;
            _organizationService = organizationService;
        }

        protected abstract BaseItem DeserializeItem(TextReader tr, JsonSerializer serializer);
        protected abstract void ProcessItem(BaseItem item);
        public void ExecuteTask(SecurityToken token, string parameters)
        {
            if (_isExecuting) return;
            _isExecuting = true;
            var maxArchiveAgeInDays = 15;
            var maxErrorAgeInMonths = 1;
            string[] args = parameters.Split(',');
            foreach (var arg in args)
            {
                var keyValue = arg.Trim().Split('=');
                if (keyValue.Length == 2)
                {
                    switch (keyValue[0].ToLower())
                    {
                        case "maxarchiveageindays":
                            if (!int.TryParse(keyValue[1], out maxArchiveAgeInDays))
                                maxArchiveAgeInDays = 15;
                            break;
                        case "maxerrorageinmonths":
                            if (!int.TryParse(keyValue[1], out maxErrorAgeInMonths))
                                maxErrorAgeInMonths = 1;
                            break;
                    }
                }
            }
            var directory = new DirectoryInfo(_path);
            if (!directory.Exists)
            {
                directory.Create();
            }

            Monitor.Enter(_lock);
            var itemId = string.Empty;
            FileInfo previousFile = null;
            var lastProcessedItemId = string.Empty;
            try
            {
                foreach (var file in directory.GetFiles().OrderBy(x => x.Name))
                {
                    var currentItemId = file.Name.IndexOf("-") > -1 ? file.Name.Split('-')[0] : string.Empty;
                    try
                    {
                        if ((string.IsNullOrEmpty(itemId) || itemId.Equals(currentItemId, StringComparison.OrdinalIgnoreCase)) && currentItemId.StartsWith("LS", StringComparison.OrdinalIgnoreCase))
                        {
                            if (string.IsNullOrEmpty(itemId) && !string.IsNullOrEmpty(currentItemId))
                            {
                                itemId = currentItemId;
                            }
                            if (previousFile != null)
                            {
                                MoveToFolder(previousFile, Path.Combine(_archive, DateTime.Now.ToShortDateString(), DateTime.Now.Hour.ToString()));
                            }
                            previousFile = file;
                            continue;
                        }
                        if (!currentItemId.StartsWith("LS", StringComparison.OrdinalIgnoreCase))
                        {
                            ProcessItem(file, itemId, out lastProcessedItemId);
                        }
                        else if (previousFile != null)
                        {
                            ProcessItem(previousFile, itemId, out lastProcessedItemId);
                        }
                    }
                    catch (Exception e)
                    {
                        _log.Error(e.Message, e);
                        var fileName = MoveToFolder(previousFile, _error);
                        CreateErrorLog(fileName, e);
                    }
                    if (currentItemId.StartsWith("LS", StringComparison.OrdinalIgnoreCase))
                    {
                        previousFile = file;
                        itemId = currentItemId;
                    }
                }

                if ((string.IsNullOrEmpty(lastProcessedItemId) || !lastProcessedItemId.Equals(itemId, StringComparison.OrdinalIgnoreCase)) && previousFile != null)
                {
                    ProcessItem(previousFile, itemId, out lastProcessedItemId);
                }

                CleanupOldFiles(_archive, DateTime.UtcNow.AddDays(-1 * maxArchiveAgeInDays));
                CleanupOldFiles(_error, DateTime.UtcNow.AddMonths(-1 * maxErrorAgeInMonths));
            }
            finally
            {
                lastProcessedItemId = string.Empty;
                itemId = string.Empty;
                previousFile = null;
                _isExecuting = false;
                Monitor.Exit(_lock);
            }
        }


        private void ProcessItem(FileInfo file, string itemId, out string lastProcessedItemId)
        {
            try
            {
                _log.Info(Messages.FileProcessing, file.Name);
                ProcessItem(ReadFile(file));
                MoveToFolder(file, Path.Combine(_archive, DateTime.Now.ToShortDateString(), DateTime.Now.Hour.ToString()));
                _log.Info(Messages.FileCompletedSucceeded);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
                var fileName = MoveToFolder(file, _error);
                CreateErrorLog(fileName, e);
            }
            lastProcessedItemId = itemId;
        }

        private void CreateErrorLog(string fileName, Exception ex)
        {
            var log = new StringBuilder();
            var exception = ex;
            while (exception != null)
            {
                log.Append("Messages: ").AppendLine(exception.Message);
                log.Append("Exception: ").AppendLine(exception.GetType().AssemblyQualifiedName);
                log.AppendLine(exception.StackTrace);
                log.AppendLine();

                exception = exception.InnerException;
            }

            var errorFile = new FileInfo(string.Format("{0}-error.txt", fileName));
            using (var stream = new StreamWriter(errorFile.OpenWrite()))
            {
                stream.Write(log.ToString());
                stream.Flush();
            }
        }

        protected void CopyFields(MultiCultureFieldContainer fieldContainer, BaseItem item)
        {
            if (item is Litium.Synchronization.Abstraction.Customers.Organization)
            {
                var organization = (Litium.Synchronization.Abstraction.Customers.Organization)item;
                CopyFields(fieldContainer, organization.Fields, organization.ParentOrganizationSystemId, organization.LegalRegistrationNumber);
            }
            if (item is Litium.Synchronization.Abstraction.Customers.Person)
            {
                var person = (Litium.Synchronization.Abstraction.Customers.Person)item;
                CopyFields(fieldContainer, person.Fields, null, null);
                var commentsValue = fieldContainer.GetValue<string>("Comments");
                if (!string.IsNullOrWhiteSpace(commentsValue) || !string.IsNullOrWhiteSpace(person.Comments))
                {
                    AssignValue(fieldContainer, "Comments", person.Comments);
                }
                AssignValue(fieldContainer, "Gender", person.IsFemale);
                var phoneHomeValue = fieldContainer.GetValue<string>("PhoneHome");
                if (!string.IsNullOrWhiteSpace(phoneHomeValue) || !string.IsNullOrWhiteSpace(person.PhoneHome))
                {
                    AssignValue(fieldContainer, "PhoneHome", person.PhoneHome);
                }
                var phoneMobileValue = fieldContainer.GetValue<string>("PhoneMobile");
                if (!string.IsNullOrWhiteSpace(phoneMobileValue) || !string.IsNullOrWhiteSpace(person.PhoneMobile))
                {
                    AssignValue(fieldContainer, "PhoneMobile", person.PhoneMobile);
                }
                var phoneWorkValue = fieldContainer.GetValue<string>("PhoneWork");
                if (!string.IsNullOrWhiteSpace(phoneWorkValue) || !string.IsNullOrWhiteSpace(person.PhoneWork))
                {
                    AssignValue(fieldContainer, "PhoneWork", person.PhoneWork);
                }
                var emailValue = fieldContainer.GetValue<string>("_email");
                if (!string.IsNullOrWhiteSpace(emailValue) || !string.IsNullOrWhiteSpace(person.Email))
                {
                    AssignValue(fieldContainer, "_email", person.Email);
                }
                var titleValue = fieldContainer.GetValue<string>("_title");
                if (!string.IsNullOrWhiteSpace(titleValue) || !string.IsNullOrWhiteSpace(person.Title))
                {
                    AssignValue(fieldContainer, "_title", person.Title);
                }
            }
        }

        private void CopyFields(MultiCultureFieldContainer fieldContainer, List<Field> fields, Guid? parentOrganizationSystemId, string legalRegistrationNumber)
        {
            foreach (var field in fields)
            {
                AssignValue(fieldContainer, field);
            }
            if (parentOrganizationSystemId.HasValue)
            {
                var parentOrganization = _organizationService.Get(parentOrganizationSystemId.Value);
                if (parentOrganization != null)
                {
                    AssignValue(fieldContainer, "ParentOrganization", parentOrganizationSystemId.Value);
                }
            }
            var currentLegalRegistrationNumber = fieldContainer.GetValue<string>("LegalRegistrationNumber");
            if (!string.IsNullOrWhiteSpace(currentLegalRegistrationNumber) || !string.IsNullOrWhiteSpace(legalRegistrationNumber))
            {
                AssignValue(fieldContainer, "LegalRegistrationNumber", legalRegistrationNumber);
            }
        }

        
        private void AssignValue(MultiCultureFieldContainer fieldContainer, string fieldDefinitionId, object value)
        {
            var fieldDefinition = _fieldDefinitionService.Get<CustomerArea>(fieldDefinitionId);
            if (fieldDefinition != null)
            {
                AddSingleValue(fieldContainer, fieldDefinitionId, value, fieldDefinition.MultiCulture);
            }
        }

        private void AssignValue(MultiCultureFieldContainer fieldContainer, Field field)
        {
            var fieldDefinition = _fieldDefinitionService.Get(field.FieldDefinitionSystemId);
            if (fieldDefinition != null)
            {
                if (fieldDefinition.FieldType.Equals(field.FieldType.ToString(), StringComparison.OrdinalIgnoreCase) || (fieldDefinition.FieldType.Equals("Date", StringComparison.OrdinalIgnoreCase) && field.FieldType == FieldType.DateTime))
                {
                    switch (field.FieldType)
                    {
                        case FieldType.Boolean:
                            AddSingleValue(fieldContainer, fieldDefinition.Id, field.BooleanValue, fieldDefinition.MultiCulture);
                            break;
                        case FieldType.Decimal:
                            AddSingleValue(fieldContainer, fieldDefinition.Id, field.DecimalValue, fieldDefinition.MultiCulture);
                            break;
                        case FieldType.DateTime:
                            AddSingleValue(fieldContainer, fieldDefinition.Id, field.DateTimeValue, fieldDefinition.MultiCulture);
                            break;
                        case FieldType.LimitedText:
                            var limitedTextValue = fieldContainer.GetValue<string>(fieldDefinition.Id);
                            if (!string.IsNullOrWhiteSpace(limitedTextValue) || !string.IsNullOrWhiteSpace(field.IndexedTextValue))
                            {
                                AddSingleValue(fieldContainer, fieldDefinition.Id, field.IndexedTextValue, fieldDefinition.MultiCulture);
                            }
                            break;
                        case FieldType.TextOption:
                            if (field.IsMultiSelect)
                            {
                                AddStringArrayValue(fieldContainer, fieldDefinition.Id, field.TextOptionValue, fieldDefinition.MultiCulture);
                            }
                            else
                            {
                                AddSingleValue(fieldContainer, fieldDefinition.Id, field.TextOptionValue.FirstOrDefault(), fieldDefinition.MultiCulture);
                            }
                            break;
                    }
                }
                else if (field.FieldType == FieldType.LimitedText && (fieldDefinition.FieldType == "Text" || fieldDefinition.FieldType == "MultirowText"))
                {
                    var textValue = fieldContainer.GetValue<string>(fieldDefinition.Id);
                    if (!string.IsNullOrWhiteSpace(textValue) || !string.IsNullOrWhiteSpace(field.IndexedTextValue))
                    {
                        AddSingleValue(fieldContainer, fieldDefinition.Id, field.IndexedTextValue, fieldDefinition.MultiCulture);
                    }
                }
            }
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

        private void AddStringArrayValue(MultiCultureFieldContainer fields, string fieldDefinitonId, List<string> values, bool isMulticulture)
        {
            if (isMulticulture)
            {
                var languages = _languageService.GetAll();
                foreach (var language in languages)
                {
                    fields.AddOrUpdateValue(fieldDefinitonId, language.CultureInfo.Name, values);
                }
            }
            else
            {
                fields.AddOrUpdateValue(fieldDefinitonId, values);
            }
        }

        private void CleanupOldFiles(string folder, DateTime olderThanUtc)
        {
            var folderPath = Path.Combine(_path, folder);
            var directory = new DirectoryInfo(folderPath);
            if (!directory.Exists)
            {
                return;
            }

            var directoriesToRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in directory.GetFiles("*.*", SearchOption.AllDirectories))
            {
                if (file.LastWriteTimeUtc < olderThanUtc)
                {
                    directoriesToRemove.Add(file.Directory.FullName);
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception e)
                    {
                        _log.Fatal(string.Format(Messages.RemoveFileError, file.FullName, e.Message), e);
                    }
                }
            }

            foreach (var subDir in directoriesToRemove
                .Where(x => !folderPath.Equals(x, StringComparison.OrdinalIgnoreCase))
                .Select(x => new DirectoryInfo(x))
                .Where(x => x.Exists && x.GetFiles().LongLength == 0 && x.GetDirectories().LongLength == 0))
            {
                try
                {
                    subDir.Delete(true);
                }
                catch (Exception e)
                {
                    _log.Fatal(string.Format(Messages.RemoveDirectoryError, subDir.FullName, e.Message), e);
                }
            }
        }

        void IDisposable.Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                _log.Fatal(string.Format(Messages.DisposeException, e.Message), e);
                throw;
            }
        }
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        ///     Moves to folder.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="folder">The folder.</param>
        private string MoveToFolder(FileInfo file, string folder)
        {
            var folderPath = Path.Combine(_path, folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
            Debug.Assert(fileNameWithoutExtension != null, "fileNameWithoutExtension != null");

            var destFileName = string.Format("{0}-{2}{1}", Path.Combine(folder, fileNameWithoutExtension), Path.GetExtension(file.Name), DateTime.Now.Ticks);

            _log.Info(Messages.MovingFile, destFileName);
            var fileName = Path.Combine(_path, destFileName);
            try
            {
                file.MoveTo(fileName);
            }
            catch (Exception e)
            {
                _log.Fatal(string.Format(Messages.MovingFileError, folder, e.Message), e);
            }
            return fileName;
        }
        private BaseItem ReadFile(FileInfo file)
        {
            var tryCount = 0;
            var autoResetEvent = new AutoResetEvent(false);
            while (tryCount < MaxRetryCount + 1) //check for maxtryCount+1 to give a chance for exception to get logged and thrown.
            {
                try
                {
                    using (var sr = file.OpenText())
                    {
                        var serializer = new JsonSerializer();
                        var item = DeserializeItem(sr, serializer);
                        if (item != null)
                        {
                            return item;
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.DeserializationError, file.Name));
                        }
                    }
                }
                catch (IOException)
                {
                    //avoid indefinite release-catch, release-catch scenario with tryCount.
                    if (tryCount > MaxRetryCount)
                    {
                        throw;
                    }
                    autoResetEvent.WaitOne(MaxWaitTime);
                }
                catch (Exception e)
                {
                    _log.Error(string.Format(Messages.ReadingFileError, file.FullName, e.Message), e);
                    throw;
                }
            }
            return null;
        }
    }
}
