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
using Ninject.Core;
using Ninject.Core.Parameters;

namespace Balder.Core.Execution
{
	[Singleton]
	public class ObjectFactory : IObjectFactory
	{
		private readonly IKernel _kernel;

		public ObjectFactory(IKernel kernel)
		{
			_kernel = kernel;
		}

		public T Get<T>()
		{
			var objectToReturn = _kernel.Get<T>();
			return objectToReturn;
		}

		public T Get<T>(params ConstructorArgument[] constructorArguments)
		{
			var arguments = CreateConstructorArgumentsDictionary(constructorArguments);
			var objectToReturn = _kernel.Get<T>(With.Parameters.ConstructorArguments(arguments));
			return objectToReturn;
		}

		public object Get(Type type)
		{
			var objectToReturn = _kernel.Get(type);
			return objectToReturn;
		}

		public object Get(Type type, params ConstructorArgument[] constructorArguments)
		{
			var arguments = CreateConstructorArgumentsDictionary(constructorArguments);
			var objectToReturn = _kernel.Get(type, With.Parameters.ConstructorArguments(arguments));
			return objectToReturn;
		}


		public void WireUpDependencies(object objectToWire)
		{
			_kernel.Inject(objectToWire);
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