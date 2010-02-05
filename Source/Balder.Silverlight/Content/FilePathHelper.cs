#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
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
using Balder.Core.Content;
using Balder.Core.Execution;
using Balder.Core.Utils;

namespace Balder.Silverlight.Content
{
	public class FilePathHelper
	{
		private readonly IContentManager _contentManager;
		private readonly IPlatform _platform;

		public FilePathHelper(IContentManager contentManager, IPlatform platform)
		{
			_contentManager = contentManager;
			_platform = platform;
		}

		public string GetFileNameForAsset(string assetName)
		{
			var fullAssemblyName = _platform.EntryAssemblyName;
			var assemblyName = AssemblyHelper.GetAssemblyShortName(fullAssemblyName);
			var filename = string.Empty;

			if( assetName.Contains(";component"))
			{
				filename = assetName;
			} else
			{
				filename = string.Format("/{0};component/{1}/{2}", assemblyName, _contentManager.AssetsRoot, assetName);	
			}

			filename = filename.Replace("\\", "/");
			filename = filename.Replace("//", "/");
			
			return filename;
		}
	}
}
