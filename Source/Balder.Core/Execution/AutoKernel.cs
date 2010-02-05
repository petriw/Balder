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
using Ninject.Core.Activation;
using Ninject.Core.Binding;
using Ninject.Core.Creation.Providers;

namespace Balder.Core.Execution
{
	public delegate IBinding BindingResolver(IContext context);
	public delegate IBinding GenericBindingResolver(Type type, IContext context);

	public class AutoKernel : StandardKernel
	{
		private readonly Dictionary<Type, BindingResolver> _bindingResolvers;
		private readonly List<GenericBindingResolver> _genericBindingResolvers;

		public AutoKernel(params IModule[] modules)
			: base(modules)
		{
			_bindingResolvers = new Dictionary<Type, BindingResolver>();
			_genericBindingResolvers = new List<GenericBindingResolver>();
		}

		public void AddBindingResolver<T>(BindingResolver resolver)
		{
			_bindingResolvers[typeof (T)] = resolver;
		}

		public void AddGenericBindingResolver(GenericBindingResolver resolver)
		{
			_genericBindingResolvers.Add(resolver);
		}



		protected override IBinding ResolveBinding(Type service, IContext context)
		{
			IBinding binding = null;
			foreach (var resolver in _genericBindingResolvers)
			{
				binding = resolver(service, context);
				if (null != binding)
				{
					break;
				}
			}

			if (null == binding)
			{
				binding = base.ResolveBinding(service, context);
				if (null == binding)
				{
					if (_bindingResolvers.ContainsKey(service))
					{
						binding = _bindingResolvers[service](context);
					}
					else
					{
						var serviceName = service.Name;
						if (serviceName.StartsWith("I"))
						{
							var instanceName = string.Format("{0}.{1}", service.Namespace, serviceName.Substring(1));
							var serviceInstanceType = service.Assembly.GetType(instanceName);
							if (null != serviceInstanceType)
							{
								if (serviceInstanceType.IsAbstract)
								{
									return null;
								}
								binding = new StandardBinding(this, service);
								var provider = new StandardProvider(serviceInstanceType);
								binding.Provider = provider;
							}
						}
					}
				}
			}

			return binding;
		}
	}
}