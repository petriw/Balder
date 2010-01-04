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


		public T Clone<T>(T node)
			where T : Node
		{
			var clone = Clone(node, true);
			return clone;
		}

		public T Clone<T>(T node, bool keepContentAsReference)
			where T:Node
		{
			var clone = _objectFactory.Get<T>();

			return clone;
		}
	}
}
