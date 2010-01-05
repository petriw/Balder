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
using Balder.Core.Content;
using Balder.Core.Display;
using Balder.Core.Imaging;
using Balder.Core.Input;
using Balder.Core.Objects.Flat;
using Balder.Core.Objects.Geometries;
using Balder.Core.Rendering;
using Ninject.Core;
using Ninject.Core.Activation;
using Ninject.Core.Behavior;
using Ninject.Core.Binding;
using Ninject.Core.Binding.Syntax;

namespace Balder.Core.Execution
{
	public class PlatformKernel : AutoKernel
	{
		public PlatformKernel(IPlatform platform)
		{
			var runtimeModule = GetRuntimeModule(platform);
			Load(runtimeModule);
			
			AddBindingResolver<IDisplay>(DisplayBindingResolver);
		}

		private IBinding DisplayBindingResolver(IContext context)
		{
			var binding = new StandardBinding(this, typeof(IDisplay));
			IBindingTargetSyntax binder = new StandardBindingBuilder(binding);

			if (null != context.ParentContext &&
				context.ParentContext is DisplayActivationContext)
			{
				var display = ((DisplayActivationContext)context.ParentContext).Display;
				binder.ToConstant(display);
			}

			return binding;
		}


		private static InlineModule GetRuntimeModule(IPlatform platform)
		{
			var module = new InlineModule(
				m => m.Bind<IPlatform>().ToConstant(platform),
				m => m.Bind<IDisplayDevice>().ToConstant(platform.DisplayDevice),
				m => m.Bind<IMouseDevice>().ToConstant(platform.MouseDevice),
				m => m.Bind<IFileLoader>().To(platform.FileLoaderType).Using<SingletonBehavior>(),
				m => m.Bind<IGeometryContext>().To(platform.GeometryContextType),
				m => m.Bind<ISpriteContext>().To(platform.SpriteContextType),
				m => m.Bind<IImageContext>().To(platform.ImageContextType),
				m => m.Bind<IShapeContext>().To(platform.ShapeContextType)
			);
			return module;
		}
	}
}
