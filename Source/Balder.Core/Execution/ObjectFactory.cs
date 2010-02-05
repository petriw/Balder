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
using System;
using System.Collections.Generic;
using Ninject.Core;
using Ninject.Core.Parameters;

namespace Balder.Core.Execution
{
	[Singleton]
	public class ObjectFactory : IObjectFactory
	{
		private static readonly object InstanceLockObject = new object();
		private static IObjectFactory _instance;

		internal static bool IsObjectFactoryInitialized
		{
			get { return null != KernelContainer.Kernel;  }
		}

		// TODO:
		// I'm not too fond of singletons, and this specific singleton I really dislike.
		// The reason why I "need" it (or rather can't seem to come up with a better way,
		// is that some objects (Geometry, Sprite and so forth) needs to be able to create
		// instances of a specific device context for their purpose, and they can't have
		// their dependencies injected on the constructor because they are on the Silverlight
		// platform also at the same time controls and must be able to be instantiated on the
		// Xaml surface, and I really would like to not have to use late injection of properties
		// as that make them not intuitive to use, especially from testing purposes.
		// So, the next best thing I can come up with is to create an internal singleton for them
		// to be able to use. Tests can use the set method to set the instance to whatever 
		// instance is appropriate for the test(s) to run.
		internal static IObjectFactory	Instance
		{
			get
			{
				lock( InstanceLockObject )
				{
					if( null == _instance )
					{
						_instance = KernelContainer.Kernel.Get<IObjectFactory>();
					}
					return _instance;
				}
			}

			set
			{
				_instance = value;
			}
		}


		public T Get<T>()
		{
			var objectToReturn = KernelContainer.Kernel.Get<T>();
			return objectToReturn;
		}

		public T Get<T>(params ConstructorArgument[] constructorArguments)
		{
			var arguments = CreateConstructorArgumentsDictionary(constructorArguments);
			var objectToReturn = KernelContainer.Kernel.Get<T>(With.Parameters.ConstructorArguments(arguments));
			return objectToReturn;
		}

		public object Get(Type type)
		{
			var objectToReturn = KernelContainer.Kernel.Get(type);
			return objectToReturn;
		}

		public object Get(Type type, params ConstructorArgument[] constructorArguments)
		{
			var arguments = CreateConstructorArgumentsDictionary(constructorArguments);
			var objectToReturn = KernelContainer.Kernel.Get(type, With.Parameters.ConstructorArguments(arguments));
			return objectToReturn;
		}


		public void WireUpDependencies(object objectToWire)
		{
			KernelContainer.Kernel.Inject(objectToWire);
		}


		private static Dictionary<string, object> CreateConstructorArgumentsDictionary(IEnumerable<ConstructorArgument> constructorArguments)
		{
			var constructorArgumentsDictionary = new Dictionary<string, object>();
			foreach( var constructorArgument in constructorArguments )
			{
				constructorArgumentsDictionary[constructorArgument.Name] = constructorArgument.Value;
			}
			return constructorArgumentsDictionary;
		}

	}
}