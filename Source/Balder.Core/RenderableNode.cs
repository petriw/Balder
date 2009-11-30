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
using Balder.Core.Display;
using Balder.Core.Math;

namespace Balder.Core
{
	public abstract class RenderableNode : Node
	{
		public void PrepareRender()
		{
			PositionMatrix[3, 0] = Position.X;
			PositionMatrix[3, 1] = Position.Y;
			PositionMatrix[3, 2] = Position.Z;
		}

		public virtual void BeforeRender() {}
		public virtual void AfterRender() {}

		#region Public Abstract Methods

		public abstract void Render(Viewport viewport, Matrix view, Matrix projection);

		public virtual void PostRender(Viewport viewport, Matrix renderMatrix, Matrix projectionMatrix)
		{
		}

		#endregion
	}
}
