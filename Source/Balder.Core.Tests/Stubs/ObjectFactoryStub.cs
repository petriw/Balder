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
using Balder.Core.Execution;

namespace Balder.Core.Tests.Stubs
{
	public class ObjectFactoryStub : IObjectFactory
	{
		public T Get<T>()
		{
			var type = typeof (T);
			var objectToCreate = Get(type);
			return (T)objectToCreate;
		}

		public T Get<T>(params ConstructorArgument[] constructorArguments)
		{
			throw new NotImplementedException();
		}

		public void WireUpDependencies(object objectToWire)
		{
			throw new NotImplementedException();
		}

		public object Get(Type type, params ConstructorArgument[] constructorArguments)
		{
			throw new NotImplementedException();
		}

		public object Get(Type type)
		{
			var objectToCreate = Activator.CreateInstance(type);
			return objectToCreate;
		}
	}
}
