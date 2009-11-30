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

namespace Balder.Core.Objects.Geometries
{
	public class Mesh : RenderableNode, IAsset
	{
		private Geometry[] _geometries;
		private readonly IAssetLoaderService _assetLoaderService;
		private readonly IDebugRenderer _debugRenderer;

		public Mesh(IAssetLoaderService assetLoaderService, IDebugRenderer debugRenderer)
		{
			_assetLoaderService = assetLoaderService;
			_debugRenderer = debugRenderer;
		}

		public void Load(string assetName)
		{
			var loader = _assetLoaderService.GetLoader<Geometry>(assetName);
			_geometries = loader.Load(assetName);

			var boundingSphere = new BoundingSphere(Vector.Zero,0);
			for( var geometryIndex=0; geometryIndex<_geometries.Length; geometryIndex++ )
			{
				var geometry = _geometries[geometryIndex];
				geometry.InitializeBoundingSphere();
				boundingSphere = BoundingSphere.CreateMerged(boundingSphere, geometry.BoundingSphere);
			}
			BoundingSphere = boundingSphere;
		}

		public override void Render(Viewport viewport, Matrix view, Matrix projection)
		{
			for( var geometryIndex=0; geometryIndex<_geometries.Length; geometryIndex++ )
			{
				var geometry = _geometries[geometryIndex];

				var localWorld = World * geometry.World * PositionMatrix;

				if (Runtime.Instance.DebugLevel.IsBoundingSpheresSet())
				{
					_debugRenderer.RenderBoundingSphere(BoundingSphere, viewport, view, projection, localWorld);
				}
				
				geometry.GeometryContext.Render(viewport,this,view,projection,localWorld);
			}
		}

		public int TotalFaceCount
		{
			get
			{
				var faceCount = 0;
				for (var geometryIndex = 0; geometryIndex < _geometries.Length; geometryIndex++)
				{
					var geometry = _geometries[geometryIndex];
					faceCount += geometry.GeometryContext.FaceCount;
				}
				return faceCount;
			}
		}
	}
}