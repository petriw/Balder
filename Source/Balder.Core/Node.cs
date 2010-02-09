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
using System.ComponentModel;
using System.Windows.Data;
using Balder.Core.Collections;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Math;
using Matrix = Balder.Core.Math.Matrix;

namespace Balder.Core
{
	/// <summary>
	/// Abstract class representing a node in a scene
	/// </summary>
	public abstract partial class Node : IAmCopyable<Node>
	{
		private static readonly EventArgs DefaultEventArgs = new EventArgs();
		public event EventHandler Hover = (s, e) => { };
		public event EventHandler Click = (s, e) => { };

		private bool _isInitializingTransform;

		protected Node()
		{
			IsVisible = true;
			InitializeTransform();
			InitializeColor();
			Initialize();
		}

		partial void Initialize();

		private void InitializeColor()
		{
			Color = Color.Random();
		}


		private void InitializeTransform()
		{
			_isInitializingTransform = true;
			Position = new Coordinate();
			PivotPoint = new Coordinate();
			Scale = new Coordinate(1f, 1f, 1f);
			Rotation = new Coordinate();
			_isInitializingTransform = false;
			Children = new NodeCollection();
			PrepareWorld();
		}


		public static readonly Property<Node, bool> IsVisibleProp = Property<Node, bool>.Register(n => n.IsVisible);
		public bool IsVisible
		{
			get { return IsVisibleProp.GetValue(this); }
			set { IsVisibleProp.SetValue(this, value); }
		}

		public BoundingSphere BoundingSphere { get; set; }
		public Scene Scene { get; set; }
		public NodeCollection Children { get; private set; }

		public static readonly Property<Node, Coordinate> PivotPointProperty = Property<Node, Coordinate>.Register(n => n.PivotPoint);
		public Coordinate PivotPoint
		{
			get { return PivotPointProperty.GetValue(this); }
			set { PivotPointProperty.SetValue(this, value); }
		}

		#region Transform
		public static readonly Property<Node, Coordinate> PositionProp =
			Property<Node, Coordinate>.Register(t => t.Position);
		private Coordinate _position;
		/// <summary>
		/// Gets or sets the position of the node in 3D space
		/// </summary>
		public Coordinate Position
		{
			get { return PositionProp.GetValue(this); }
			set
			{
				if (null != _position)
				{
					_position.PropertyChanged -= TransformChanged;
				}
				PositionProp.SetValue(this, value);
				_position = value;
				if (null != _position)
				{
					_position.PropertyChanged += TransformChanged;
					PrepareWorld();
				}
			}
		}


		public static readonly Property<Node, Coordinate> ScaleProp =
			Property<Node, Coordinate>.Register(t => t.Scale);

		private Coordinate _scale;

		/// <summary>
		/// Gets or sets the scale of the node
		/// </summary>
		/// <remarks>
		/// Default is X:1 Y:1 Z:1, which represents the node in a non-scaled form
		/// </remarks>
		public Coordinate Scale
		{
			get { return ScaleProp.GetValue(this); }
			set
			{
				if (null != _scale)
				{
					_scale.PropertyChanged -= TransformChanged;
				}
				ScaleProp.SetValue(this, value);
				_scale = value;
				if (null != _scale)
				{
					_scale.PropertyChanged += TransformChanged;
					PrepareWorld();
				}
			}
		}

		public static readonly Property<Node, Coordinate> RotationProp =
			Property<Node, Coordinate>.Register(t => t.Rotation);

		private Coordinate _rotation;

		/// <summary>
		/// Gets or sets the rotation of the node in angles, 0-360 degrees
		/// </summary>
		public Coordinate Rotation
		{
			get { return RotationProp.GetValue(this); }
			set
			{
				if (null != _rotation)
				{
					_rotation.PropertyChanged -= TransformChanged;
				}
				RotationProp.SetValue(this, value);
				_rotation = value;
				if (null != _rotation)
				{
					_rotation.PropertyChanged += TransformChanged;
					PrepareWorld();
				}
			}
		}

		public Matrix World { get; set; }

		public Matrix RenderingWorld { get; internal set; }

		private void PrepareWorld()
		{
			if (_isInitializingTransform)
			{
				return;
			}
			var scaleMatrix = Matrix.CreateScale(Scale);
			var translationMatrix = Matrix.CreateTranslation(Position);
			var rotationMatrix = Matrix.CreateRotation((float)Rotation.X, (float)Rotation.Y, (float)Rotation.Z);
			World = rotationMatrix * scaleMatrix * translationMatrix;
		}

		private void TransformChanged(object sender, PropertyChangedEventArgs e)
		{
			PrepareWorld();
		}
		#endregion

		public virtual void PrepareForRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world) { }

		internal void OnHover()
		{
			Hover(this, DefaultEventArgs);
		}

		internal void OnClick()
		{
			Click(this, DefaultEventArgs);
		}

		protected void SetColorForChildren()
		{
			foreach (var node in Children)
			{
				node.Color = Color;
			}
		}


		public virtual void CopyFrom(Node source)
		{
			BoundingSphere = source.BoundingSphere;
			Position.Set(source.Position);
			Scale.Set(source.Scale);
			Rotation.Set(source.Rotation);
			Color = source.Color;
			Command = source.Command;
			CommandParameter = source.CommandParameter;
			PrepareWorld();
		}


		public virtual Node Clone()
		{
			var clone = NodeCloner.Instance.Clone(this);
			clone.IsClone = true;
			return clone as Node;
		}

		protected bool IsClone { get; private set; }
	}
}
