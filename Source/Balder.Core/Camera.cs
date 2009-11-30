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
	public class Camera : Node
	{
		public const float DefaultFieldOfView = 40f;
		public const float DefaultFar = 4000f;
		public const float DefaultNear = 1f;

		private float _fieldOfView;
		private float _near;
		private float _far;


		#region Constructor(s)
		public Camera(Viewport viewport)
		{
			Position = new Vector(0f, -30f, 50f);
			Target = new Vector(0f, 0f, 0f);
            Up = new Vector(0f, -1f, 0f);
			Near = DefaultNear;
			Far = DefaultFar;
			Roll = 0;
			FieldOfView = DefaultFieldOfView;
			ProjectionMatrix = null;
			UpdateDepthDivisor();

			Frustum = new Frustum();

			Viewport = viewport;
		}
		#endregion

		#region Public Properties

		public Viewport Viewport { get; private set; }

		public Frustum Frustum { get; private set; }

		public Matrix ViewMatrix { get; private set; }
		public Matrix ProjectionMatrix { get; private set; }

		/// <summary>
		/// Get and set the target for the Camera - The location the camera is looking at
		/// </summary>
		public Vector Target;

		/// <summary>
		/// Get the forward vector for the camera. This is calculated from the target and position
		/// </summary>
		public Vector Forward
		{
			get { return Target - Position; }
		}


		/// <summary>
		/// Get the up vector for the camera. Whenever you change the roll, this vector changes!
		/// </summary>
		public Vector Up { get; private set; }


		/// <summary>
		/// Gets or sets the roll of the camera. 
		/// </summary>
		public float Roll { get; set; }

		/// <summary>
		/// Gets or sets the near distance clipping plane
		/// </summary>
		public float Near
		{
			get { return _near; }
			set
			{
				_near = value;
				UpdateDepthDivisor();
			}
		}

		/// <summary>
		/// Gets or sets the far distance clipping plane
		/// </summary>
		public float Far
		{
			get { return _far; }
			set
			{
				_far = value;
				UpdateDepthDivisor();
			}
		}

		/// <summary>
		/// Gets the divisor used for transforming Z values for purposes such as depth buffers
		/// </summary>
		public float DepthDivisor { get; private set; }

		/// <summary>
		/// Gets the value that indicates the actual zero/start of the depth, typically used by depth buffers
		/// </summary>
		public float DepthZero { get; private set; }



		/// <summary>
		/// Gets or sets the field of view for the camera
		/// </summary>
		public float FieldOfView
		{
			get
			{
				return _fieldOfView;
			}
			set
			{
				_fieldOfView = value;
				SetupProjection();
			}
		}

		public float AspectRatio { get { return ((float) Viewport.Width)/((float) Viewport.Height); } }
		#endregion

		#region Private Methods

		/// <summary>
		/// Calculates the projection matrix
		/// </summary>
		private void SetupProjection()
		{
			if( null == Viewport )
			{
				return;
			}
			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
				MathHelper.ToRadians(FieldOfView), 
				AspectRatio, 
				Near, 
				Far);
		}

		private void UpdateDepthDivisor()
		{
			DepthDivisor = Far - Near;
			DepthZero = Near/DepthDivisor;
			SetupProjection();
		}
		#endregion

		#region Public Methods

		public override void Prepare(Viewport viewport)
		{
			Viewport = viewport;
			SetupProjection();
			base.Prepare(viewport);
		}

		public override void Update()
		{
			//Up = new Vector(-(float)System.Math.Sin(Roll), (float)System.Math.Cos(Roll), Up.Z);
			Up = new Vector(0,-1,0);
			ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
			SetupProjection();
			Frustum.SetCameraDefinition(this);
		}


		public bool IsVectorVisible(Vector vector)
		{
			return Frustum.IsPointInFrustum(vector) == FrustumIntersection.Inside;
		}

		public bool IsPointVisible(float x, float y, float z)
		{
			return Frustum.IsPointInFrustum(new Vector(x, y, z)) == FrustumIntersection.Inside;
		}

		#endregion
	}
}
