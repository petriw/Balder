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
	public abstract class Node : EngineObject
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

		/// <summary>
		/// Get and set the position in space for the node
		/// </summary>
		public Vector Position;

		/// <summary>
		/// Get and set the scale of the node
		/// </summary>
		public Vector Scale;

		/// <summary>
		/// Get and set the matrix representing the node in the world
		/// </summary>
		public Matrix World;

		/// <summary>
		/// Get and set the name of the node
		/// </summary>
		public string Name;

		/// <summary>
		/// The bounding sphere surrounding the node
		/// </summary>
		public BoundingSphere BoundingSphere;

		/// <summary>
		/// Get and set wether or not the node is visible
		/// </summary>
		public bool IsVisible { get; set; }

		public Scene Scene { get; set; }

		/// <summary>
		/// Color of the node - this will be used if node supports it
		/// during lighting calculations. If Node has different ways of defining
		/// its color, for instance Materialing or similar - this color
		/// will most likely be overridden
		/// </summary>
		public Color Color { get; set; }
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
