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
using Balder.Core.Execution;
using Balder.Core.Objects.Geometries;

namespace Balder.Core.Content
{
	public class ContentCreator
	{
		private readonly IObjectFactory _objectFactory;

		public ContentCreator(IObjectFactory objectFactory)
		{
			_objectFactory = objectFactory;
		}

		public T CreateGeometry<T>() where T : Geometry
		{
			var geometry = _objectFactory.Get<T>();
			return geometry;
		}


		public Box CreateBox()
		{
			throw new NotImplementedException();
		}

		public Geometry CreateSphere()
		{
			throw new NotImplementedException();
		}

		public Cylinder CreateCylinder(float radius, float height, int segments, int heightSegments)
		{
			var cylinder = _objectFactory.Get<Cylinder>(
					new[]
						{
							new ConstructorArgument { Name="radius", Value=radius},
							new ConstructorArgument { Name="height", Value=height},
							new ConstructorArgument { Name="segments", Value=segments},
							new ConstructorArgument { Name="heightSegments", Value=heightSegments},
						}
				);
			return cylinder;
		}
	}
}
