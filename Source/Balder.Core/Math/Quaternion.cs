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
namespace Balder.Core.Math
{
    public struct Quaternion
    {
		public float W;
        public float X;
        public float Y;
        public float Z;

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Quaternion(Quaternion Quaternion)
        {
            X = Quaternion.X;
            Y = Quaternion.Y;
            Z = Quaternion.Z;
            W = Quaternion.W;
        }
		/* this is el loco.. a float 3 isn't 4 long...
        /// <summary>
        /// Converts a Quaternion in float[3] form into the Quaternion structure
        /// </summary>
        /// <param name="quat"></param>
        public Quaternion(Vector3 quat)
        {
            x = quat[1];
            y = quat[2];
            z = quat[3];
            w = quat[0];
        }
		 * */

        /// <summary>
        /// Converts a vector & a w value to a Quaternion
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="w"></param>
        public Quaternion(Vector vector, float w)
        {
            //x = vector[0];
            //y = vector[1];
            //z = vector[2];
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            W = w;
        }

        /// <summary>
        /// Quaternion -> matrix
        /// </summary>
        /// <param name="Quaternion"></param>
        /// <returns></returns>
        public static explicit operator Matrix(Quaternion Quaternion)
        {
            Matrix Result = new Matrix();
            float x2 = Quaternion.X + Quaternion.X;
            float y2 = Quaternion.Y + Quaternion.Y;
            float z2 = Quaternion.Z + Quaternion.Z;
            float xx = Quaternion.X * x2;
            float xy = Quaternion.X * y2;
            float xz = Quaternion.X * z2;
            float yy = Quaternion.Y * y2;
            float yz = Quaternion.Y * z2;
            float zz = Quaternion.Z * z2;
            float wx = Quaternion.W * x2;
            float wy = Quaternion.W * y2;
            float wz = Quaternion.W * z2;

            Result[0, 0] = 1 - yy - zz;
            Result[1, 0] = xy - wz;
            Result[2, 0] = xz + wy;
            Result[3, 0] = 0;

            Result[0, 1] = xy + wz;
            Result[1, 1] = 1 - xx - zz;
            Result[2, 1] = yz - wx;
            Result[3, 1] = 0;

            Result[0, 2] = xz - wy;
            Result[1, 2] = yz + wx;
            Result[2, 2] = 1 - xx - yy;
            Result[3, 2] = 0;

            Result[0, 3] = 0;
            Result[1, 3] = 0;
            Result[2, 3] = 0;
            Result[3, 3] = 1;

            return Result;
        }


        public static Quaternion operator *(Quaternion A, Quaternion B)
        {
            return new Quaternion
                (
                /*C.x = */A.W * B.X + A.X * B.W + A.Y * B.Z - A.Z * B.Y,
                /*C.y = */A.W * B.Y - A.X * B.Z + A.Y * B.W + A.Z * B.X,
                /*C.z = */A.W * B.Z + A.X * B.Y - A.Y * B.X + A.Z * B.W,
                /*C.w = */A.W * B.W - A.X * B.X - A.Y * B.Y - A.Z * B.Z
            );
        }

        public Quaternion Conjugate()
        {
            return new Quaternion(-X, -Y, -Z, W);
        }

        public float Magnitude()
        {
            return Core.Sqrt(W * W + X * X + Y * Y + Z * Z);
        }

        public Quaternion Normalize()
        {
            float square = Magnitude();

            return new Quaternion(X / square, Y / square, Z / square, W / square);
        }


        public static Quaternion Slerp(Quaternion Start, Quaternion Dest, float Time)
        {
            // calc cosine
            float cosom = Start.X * Dest.X +
                     Start.Y * Dest.Y +
                     Start.Z * Dest.Z +
                     Start.W * Dest.W;

            Quaternion to1;
            // adjust signs (if necessary)
            if (cosom < 0.0)
            {
                cosom = -cosom;
                to1.X = -Dest.X;
                to1.Y = -Dest.Y;
                to1.Z = -Dest.Z;
                to1.W = -Dest.W;
            }
            else
            {
                to1.X = Dest.X;
                to1.Y = Dest.Y;
                to1.Z = Dest.Z;
                to1.W = Dest.W;
            }

            float scale0;
            float scale1;
            // calculate coefficients
            if (1 - cosom > 1e-12)
            {
                // standard case (slerp)
                float omega = (float)System.Math.Acos(cosom);
                float sinom = (float)System.Math.Sin(omega);
                scale0 = (float)(System.Math.Sin((1 - Time) * omega) / sinom);
                scale1 = (float)(System.Math.Sin(Time * omega) / sinom);
            }
            else
            {
                // "from" and "to" quaternions are very close
                // so we can do a linear interpolation
                scale0 = 1 - Time;
                scale1 = Time;
            }

            // calculate final values
            Quaternion q2;
            q2.X = (scale0 * Start.X) + (scale1 * to1.X);
            q2.Y = (scale0 * Start.Y) + (scale1 * to1.Y);
            q2.Z = (scale0 * Start.Z) + (scale1 * to1.Z);
            q2.W = (scale0 * Start.W) + (scale1 * to1.W);


            return q2.Normalize();
        }

        public static Quaternion IdentityMultiplication()
        {
            return new Quaternion(0f,0f,0f,1f);
        }
    }
}

