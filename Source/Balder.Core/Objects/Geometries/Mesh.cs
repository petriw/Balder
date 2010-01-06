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
using Balder.Core.Assets;
using Balder.Core.Debug;
using Balder.Core.Display;
using Balder.Core.Math;
using Ninject.Core;

namespace Balder.Core.Objects.Geometries
{
	public partial class Mesh : RenderableNode, IAsset
	{
		[Inject]
		public IAssetLoaderService AssetLoaderService { get; set; }

		[Inject]
		public IDebugRenderer DebugRenderer { get; set; }

		public void Load(string assetName)
		{
			var loader = AssetLoaderService.GetLoader<Geometry>(assetName);
			var geometries = loader.Load(assetName);

			var boundingSphere = new BoundingSphere(Vector.Zero,0);
			foreach( var geometry in geometries )
			{
				geometry.InitializeBoundingSphere();
				boundingSphere = BoundingSphere.CreateMerged(boundingSphere, geometry.BoundingSphere);
				Children.Add(geometry);
			}
			BoundingSphere = boundingSphere;

			// Todo: This has to be done since Loading of the node is done after Xaml has been bound - but we will get color from the File loaded
			SetColorForChildren();
		}

		
		public override void Render(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
		}
	}
}