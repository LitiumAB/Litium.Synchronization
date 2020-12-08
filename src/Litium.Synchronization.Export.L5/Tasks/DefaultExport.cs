using Litium.Foundation.Security;
using Litium.Foundation.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Litium.Owin.Logging;
using System.IO;
using System.Configuration;
using Litium.ComponentModel;
using Litium.Foundation.Configuration;
using Litium.Synchronization.Abstraction;
using System.Threading;
using Litium.Runtime;

namespace Litium.Synchronization.Export.L5.Tasks
{
    public abstract class DefaultExport 
    {
        private const int MaxRetryCount = 100; // 100ms x 20 times = 2 sec wait time for each file.
        private const int MaxWaitTime = 20; //20 ms.
        protected readonly string _path;
        protected readonly ILog _log;

        protected DefaultExport(string directory)
        {
            _log = GetType().Log();
            _path = Path.Combine(ConfigurationManager.AppSettings["SynchronizationExportFolder"].NullIfEmpty() ?? Path.Combine(GeneralConfig.Instance.CommonFilesDirectory, "SynchronizationExport"), directory);
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }

        protected abstract string GetFileName(BaseItem item);
        protected abstract void SerializeItem(TextWriter textWriter, JsonSerializer serializer, BaseItem item);
        protected virtual void WriteFile(BaseItem item)
        {
            if (item != null)
            {
                var tryCount = 0;
                var autoResetEvent = new AutoResetEvent(false);
                var fileName = Path.Combine(_path, GetFileName(item));
                if (_path.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                {
                   throw new Exception(Messages.WritingFileErrorNoFileName);
                }
                _log.Info(Messages.WritingFile, fileName);
                while (tryCount < MaxRetryCount + 1)
                {
                    try
                    {
                        tryCount++;
                        //write to disk.                    
                        using (TextWriter fs = new StreamWriter(fileName))
                        {
                            SerializeItem(fs, new JsonSerializer(), item);
                            fs.Close();
                        }
                        break;
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
                        _log.Error(string.Format(Messages.WritingFileError, fileName, e.Message), e);
                        throw;
                    }
                }
            }
        }
    }
}
