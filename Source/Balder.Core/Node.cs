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
using System;
using Matrix=Balder.Core.Math.Matrix;

namespace Balder.Core
{
	/// <summary>
	/// Abstract class representing a node in a scene
	/// </summary>
	public abstract partial class Node
	{
		private static readonly EventArgs DefaultEventArgs = new EventArgs();
		public event EventHandler Hover = (s, e) => { };
		public event EventHandler Click = (s, e) => { };

		#region Constructor(s)
		protected Node()
		{
			World = Matrix.Identity;

			PositionMatrix = Matrix.Identity;
			ScaleMatrix = Matrix.Identity;
			Scale = new Vector(1f,1f,1f);
			Position = Vector.Zero;
		}
		#endregion

		#region Public Properties


		public Scene Scene { get; set; }

		#endregion

		protected Matrix PositionMatrix { get; private set; }
		protected Matrix ScaleMatrix { get; private set; }

		public virtual void Prepare(Viewport viewport) {}
		public virtual void Update() {}


		internal void OnHover()
		{
			Hover(this, DefaultEventArgs);
		}

		internal void OnClick()
		{
			Click(this, DefaultEventArgs);
		}
	}
}
