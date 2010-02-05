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
using Ninject.Core;
using Ninject.Core.Binding;
using Ninject.Core.Binding.Syntax;

namespace Balder.Core.Utils
{
	public static class NinjectExtensions
	{
		public static IBindingTargetSyntax Bind(this IKernel kernel, Type service)
		{
			var binding = new StandardBinding(kernel, service);
			IBindingTargetSyntax binder = new StandardBindingBuilder(binding);
			kernel.AddBinding(binding);
			return binder;
		}

		public static IBindingTargetSyntax Bind<T>(this IKernel kernel)
		{
			var binding = new StandardBinding(kernel, typeof(T));
			IBindingTargetSyntax binder = new StandardBindingBuilder(binding);
			kernel.AddBinding(binding);
			return binder;
		}

		public static void Bind<T>(this IKernel kernel, T obj)
		{
			var binding = new StandardBinding(kernel, typeof(T));
			IBindingTargetSyntax binder = new StandardBindingBuilder(binding);
			binder.ToConstant<T>(obj);
			kernel.AddBinding(binding);
		}
	}
}