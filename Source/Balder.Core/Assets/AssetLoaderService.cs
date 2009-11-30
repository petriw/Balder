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
using System.IO;
using System.Linq;
using System.Reflection;
using Balder.Core.Execution;
using Balder.Core.Utils;
using Ninject.Core;

namespace Balder.Core.Assets
{
	[Singleton]
	public class AssetLoaderService : IAssetLoaderService
	{
		private readonly IObjectFactory _objectFactory;
		private readonly Dictionary<string, IAssetLoader> _assetLoaders;

		public AssetLoaderService(IObjectFactory objectFactory)
		{
			_objectFactory = objectFactory;
			_assetLoaders = new Dictionary<string, IAssetLoader>();
		}

		public void Initialize()
		{
			var assembly = typeof(AssetLoaderService).Assembly;
			var shortName = AssemblyHelper.GetAssemblyShortName(assembly.FullName);

			// Todo: Bad bad bad - the name of the assembly should be discovered automatically!
			var loaderNamespace = string.Format("Balder.Core.Assets.AssetLoaders");

			RegisterNamespace(typeof(AssetLoaderService).Assembly, loaderNamespace);
		}

		public void RegisterAssembly(string fullyQualifiedName)
		{
			var asm = Assembly.Load(fullyQualifiedName);
			RegisterAssembly(asm);
		}


		public void RegisterAssembly(Assembly assembly)
		{
			var types = assembly.GetTypes();
			var baseType = typeof(AssetLoader<>);

			var query = from t in types
			            where	null != t.BaseType &&
								t.BaseType.Name.Equals(baseType.Name)
			            select t;

			foreach (var type in query)
			{
				var loader = _objectFactory.Get(type) as IAssetLoader;
				RegisterLoader(type, loader);
			}
		}

		public void RegisterNamespace(Assembly assembly, string ns)
		{
			RegisterNamespace(assembly,ns,false);
		}

		public void RegisterNamespace(Assembly assembly, string ns, bool recursive)
		{
			var types = assembly.GetTypes();
			var baseType = typeof(AssetLoader<>);

			Func<Type,bool>   nsCheck;
			
			if( recursive )
			{
				nsCheck = (t) => t.Namespace.StartsWith(ns);
			} else
			{
				nsCheck = (t) => t.Namespace.Equals(ns);
			}

			var query = from t in types
			            where null != t.Namespace && nsCheck(t) && t.BaseType.Name.Equals(baseType.Name)
			            select t;

			foreach (var type in query)
			{
				var loader = _objectFactory.Get(type) as IAssetLoader;
				RegisterLoader(type, loader);
			}
		}

		private void RegisterLoader(Type type, IAssetLoader loader)
		{
			foreach (var extension in loader.FileExtensions)
			{
				_assetLoaders[extension.ToLower()] = loader;
			}
		}

		public void RegisterLoader<T>(AssetLoader<T> loader)
			where T:IAssetPart
		{
			RegisterLoader(typeof(T),loader);
		}

		public AssetLoader<T> GetLoader<T>(string assetName)
			where T:IAssetPart
		{
			var extension = Path.GetExtension(assetName).ToLower();
			extension = extension.Substring(1);

			if( !_assetLoaders.ContainsKey(extension))
			{
				throw new ArgumentException("There is no loader for the specified file type '"+extension+"'");
			}

			return _assetLoaders[extension] as AssetLoader<T>;
		}


		public IAssetLoader[]	AvailableLoaders
		{
			get
			{
				return _assetLoaders.Values.ToArray();
			}
		}
	}
}