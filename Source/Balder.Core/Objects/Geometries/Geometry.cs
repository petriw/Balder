﻿#region License
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
using Balder.Core.Assets;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Materials;
using Balder.Core.Math;

namespace Balder.Core.Objects.Geometries
{
	public class Geometry : RenderableNode, IAssetPart
	{
		public IGeometryContext GeometryContext { get; set; }

		private bool _materialSet = false;


		public Geometry()
		{
			// Todo : This should not be necessary.
			if (ObjectFactory.IsObjectFactoryInitialized)
			{
				GeometryContext = ObjectFactory.Instance.Get<IGeometryContext>();
			}
		}


		protected override void Initialize()
		{
			// Todo : This should not be necessary.
			if (null == GeometryContext)
			{
				GeometryContext = ObjectFactory.Instance.Get<IGeometryContext>();
			}
			
			base.Initialize();
		}


		public void InitializeBoundingSphere()
		{
			var lowestVector = Vector.Zero;
			var highestVector = Vector.Zero;
			var vertices = GeometryContext.GetVertices();
			for (var vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++)
			{
				var vector = vertices[vertexIndex].Vector;
				if (vector.X < lowestVector.X)
				{
					lowestVector.X = vector.X;
				}
				if (vector.Y < lowestVector.Y)
				{
					lowestVector.Y = vector.Y;
				}
				if (vector.Z < lowestVector.Z)
				{
					lowestVector.Z = vector.Z;
				}
				if (vector.X > highestVector.X)
				{
					highestVector.X = vector.X;
				}
				if (vector.Y > highestVector.Y)
				{
					highestVector.Y = vector.Y;
				}
				if (vector.Z > highestVector.Z)
				{
					highestVector.Z = vector.Z;
				}
			}

			var length = highestVector - lowestVector;
			var center = lowestVector + (length / 2);

			BoundingSphere = new BoundingSphere(center, length.Length / 2);
		}


		protected override void Render(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if( null != Material && !_materialSet )
			{
				GeometryContext.SetMaterialForAllFaces(Material);

				foreach( var child in Children )
				{
					if( child is Geometry )
					{
						((Geometry) child).Material = Material;
					}
				}

				_materialSet = true;
			}
			GeometryContext.Render(viewport, this, view, projection, world);
		}


		public Property<Geometry, Material> MaterialProperty = Property<Geometry, Material>.Register(g => g.Material);
		public Material Material
		{
			get { return MaterialProperty.GetValue(this); }
			set
			{
				MaterialProperty.SetValue(this, value);
				_materialSet = false;
			}
		}


#if(!SILVERLIGHT)
		public string Name { get; set; }
#endif
	}
}