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
    using FileSystem;

    public class ProjectFileBasedConfiguration :
		FileBasedConfiguration,
		ProjectConfiguration
	{
		readonly GlobalConfiguration _globalConfiguration;

		public ProjectFileBasedConfiguration(FileSystem fileSystem, GlobalConfiguration globalConfiguration)
			: base(fileSystem, fileSystem.ProjectConfig)
		{
			_globalConfiguration = globalConfiguration;

			OnMissing = GetGlobalConfigurationValue;
		}

		string GetGlobalConfigurationValue(string key)
		{
			return _globalConfiguration[key];
		}
	}
}