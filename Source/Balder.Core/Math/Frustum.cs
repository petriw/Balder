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
namespace Balder.Core.Math
{
	public enum FrustumLocation {
		Top = 0,
		Bottom,
		Left,
		Right,
		Near,
		Far,
		Total
	};

	public enum FrustumIntersection
	{
		Outside=1,
		Intersect,
		Inside
	}


	public class Frustum
	{
		private const float AngleToRadians = (float)System.Math.PI / 180.0f;
		private readonly Plane[] _planes = new Plane[(int)FrustumLocation.Total];
		private Vector ntl, ntr, nbl, nbr, ftl, ftr, fbl, fbr;
		private float _near;
		private float _far;
		private float _ratio;
		private float _angle;
		private float _tang;
		private float _nw;
		private float _nh;
		private float _fw;
		private float _fh;


		public Frustum()
		{
			for (var planeIndex = 0; planeIndex < _planes.Length; planeIndex++)
			{
				_planes[planeIndex] = new Plane();
			}
		}


		public void SetCameraInternals(float angle, float ratio, float near, float far)
		{
			_angle = angle;
			_ratio = ratio;
			_near = near;
			_far = far;

			_tang = (float)System.Math.Tan(angle * AngleToRadians * 0.5);
			_nh = near * _tang;
			_nw = _nh * ratio;
			_fh = far * _tang;
			_fw = _fh * ratio;

			
		}


		public void SetCameraDefinition(Camera camera)
		{
			Vector dir, nc, fc, X, Y, Z;

			SetCameraInternals(45, camera.AspectRatio, camera.Near, camera.Far);

			Z = camera.Position - camera.Target;
			Z.Normalize();

			X = camera.Up * Z;
			X.Normalize();

			Y = Z * X;

			nc = camera.Position - Z * _near;
			fc = camera.Position - Z * _far;

			ntl = nc + Y * _nh - X * _nw;
			ntr = nc + Y * _nh + X * _nw;
			nbl = nc - Y * _nh - X * _nw;
			nbr = nc - Y * _nh + X * _nw;

			ftl = fc + Y * _fh - X * _fw;
			ftr = fc + Y * _fh + X * _fw;
			fbl = fc - Y * _fh - X * _fw;
			fbr = fc - Y * _fh + X * _fw;


			_planes[(int)FrustumLocation.Top].SetVectors(ntr, ntl, ftl);
			_planes[(int)FrustumLocation.Bottom].SetVectors(nbl, nbr, fbr);
			_planes[(int)FrustumLocation.Left].SetVectors(ntl, nbl, fbl);
			_planes[(int)FrustumLocation.Right].SetVectors(nbr, ntr, fbr);
			_planes[(int)FrustumLocation.Near].SetVectors(ntl, ntr, fbr);
			_planes[(int)FrustumLocation.Far].SetVectors(ftr, ftl, fbl);
		}



		public FrustumIntersection IsPointInFrustum(Vector vector)
		{
			for( var planeIndex=0; planeIndex<_planes.Length; planeIndex++ )
			{
				var plane = _planes[planeIndex];
				var distance = plane.GetDistanceFromVector(vector);
                if ( distance < 0)
                {
                    return FrustumIntersection.Outside;
                }
			}
            return FrustumIntersection.Inside;

		}

		public FrustumIntersection IsSphereInFrustum(Vector vector, float radius)
		{
            float distance = 0;
			for (var planeIndex = 0; planeIndex < _planes.Length; planeIndex++)
			{
				var plane = _planes[planeIndex];
				distance = plane.GetDistanceFromVector(vector);
                if (distance < (-radius))
                {
                    return FrustumIntersection.Outside;
                }
                else if (distance < radius)
                {
                    return FrustumIntersection.Intersect;
                }
            }
            return FrustumIntersection.Inside;
		}
	}
}
