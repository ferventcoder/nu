// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace nu.core.Configuration
{
	using System;
	using FileSystem;
	using Magnum.Logging;
	using NDepend.Helpers.FileDirectoryPath;

    public class FileBasedConfiguration
	{
		static readonly ILogger _log = Logger.GetLogger<FileBasedConfiguration>();
		BasePath _configurationPath;
		bool _disposed;
		bool _touched;
		protected Func<string, string> OnMissing = DefaultMissingKeyHandler;

		protected FileBasedConfiguration(FileSystem fileSystem, FilePath configurationPath)
		{
			FileSystem = fileSystem;

			Entries = ReadExistingConfigurationFromFile(configurationPath);
		}

		public virtual string this[string key]
		{
			get
			{
				Entry entry = Entries.Get(key);
				if (entry != null)
					return entry.Value;

				return OnMissing(key);
			}

            set
            {
                Entries.Get(key, x =>
                    {
                        x.SetValue(value);
                        _touched = true;
                    });
            }
		}

        public void Remove(string key)
        {
            Entries.Remove(key);
            _touched = true;
        }

		protected FileSystem FileSystem { get; private set; }

		Entries Entries { get; set; }

        public virtual void ForEach(Action<string, string> action)
        {
            foreach (var entry in Entries)
            {
                action(entry.Key, entry.Value);
            }
        }
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool Contains(string key)
		{
			return Entries.Contains(key);
		}

		void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			if (disposing)
			{
				if (_touched)
					WriteConfigurationToFile();
			}

			_disposed = true;
		}

		Entries ReadExistingConfigurationFromFile(BasePath configurationPath)
		{
			_configurationPath = configurationPath;

			if (!FileSystem.FileExists(configurationPath.Path))
			{
				_log.Debug(x => x.Write("No existing configuration file found: {0}", configurationPath.Path));

				return new Entries();
			}

			return new Entries(JsonUtil.Get<Entry[]>(FileSystem.ReadToEnd(configurationPath.Path)) ?? new Entry[0]);
		}

		void WriteConfigurationToFile()
		{
			_log.Debug(x => x.Write("Saving configuration file: {0}", _configurationPath.Path));

			string json = JsonUtil.ToJson(Entries);

			FileSystem.Write(_configurationPath.Path, json);
		}

		~FileBasedConfiguration()
		{
			Dispose(false);
		}

		protected static string DefaultMissingKeyHandler(string key)
		{
            //_log.Warn(x => x.Write("No configuration entry for '{0}' was found", key));
		    return null;
		}
	}
}