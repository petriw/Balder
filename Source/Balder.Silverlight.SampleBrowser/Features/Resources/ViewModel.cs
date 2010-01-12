#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Windows;

namespace Balder.Silverlight.SampleBrowser.Features.Resources
{
	
	public class ViewModel
	{
		public ViewModel()
		{
			ResourceFiles = new ObservableCollection<ResourceFile>();
		}

		public void SetUrl(Uri uri)
		{
			GetNamespaceFromUri(uri);
		}

		public virtual ObservableCollection<ResourceFile> ResourceFiles { get; private set; }

		public virtual ResourceFile SelectedFile { get; set; }


		private void GetNamespaceFromUri(Uri uri)
		{
			ResourceFiles.Clear();
			var uriString = uri.OriginalString;
			var fileNameIndex = uriString.LastIndexOf('/');
			var path = uriString.Substring(0, fileNameIndex);
			if( path.StartsWith("/"))
			{
				path = path.Substring(1);
			}
			var namespaceString = path.Replace('/', '.');
			var assembly = Application.Current.GetType().Assembly;
			var assemblyName = "Balder.Silverlight.SampleBrowser";

			var fullNamespace = string.Format("{0}.{1}", assemblyName, namespaceString);

			var resources = assembly.GetManifestResourceNames();

			var query = from r in resources
						where r.StartsWith(fullNamespace)
			            select r;
			var resource = query.SingleOrDefault();
			if( !string.IsNullOrEmpty(resource))
			{
				var resourceNameWithoutExtension = resource.Replace(".resources", string.Empty);
				var resourceManager = new ResourceManager(resourceNameWithoutExtension, assembly);
				var resourceSet = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);
				var enumerator = resourceSet.GetEnumerator();
				while( enumerator.MoveNext())
				{
					var resourceName = enumerator.Key.ToString();
					var extension = string.Empty;
					var fileName = string.Empty;
					var extensionIndex = resourceName.IndexOf('_');
					if( extensionIndex > 0 )
					{
						extension = resourceName.Substring(extensionIndex + 1);
						fileName = resourceName.Substring(0, extensionIndex);
					} else
					{
						fileName = resourceName;
					}
					extension = extension.Replace('_', '.');
					var file = new ResourceFile
					           	{
					           		Filename = string.IsNullOrEmpty(extension)
					           		           	? fileName
					           		           	: string.Format("{0}.{1}", fileName, extension),
					           		Content = enumerator.Value.ToString()
					           	};
					ResourceFiles.Add(file);
				}
			}

			if( ResourceFiles.Count > 0 )
			{
				SelectedFile = ResourceFiles[0];
			}
		}
	}
}
