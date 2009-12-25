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
    public class Matrix
    {
        public float[] data;

		public Matrix()
		{
			Initialize();
		}

        public Matrix(float[] Matrix)
        {
			Initialize();
			for (var i = 0; i < data.Length; i++)
			{
				data[i] = Matrix[i];
			}
        }

		private void Initialize()
		{
			data = new float[4 * 4];
		}


		private static void SetIdentity(Matrix matrix)
		{
			for (int i = 0; i < matrix.data.Length; i++)
			{
				matrix.data[i] = 0;
			}

			for (int i = 0; i < 4; i++)
			{
				matrix[i, i] = 1;
			}
		}

        public float this[int i, int j]
        {
            get
            {
				if (data == null)
				{
					data = new float[4*4];
				}
            	return data[(i << 2) + j];
            }
            set
            {
				if (data == null)
				{
					data = new float[4*4];
				}
            	data[(i << 2) + j] = value;
            }
        }

        public static explicit operator float[](Matrix value)
        {
            return value.data;
        }


		public static Matrix operator +(Matrix matrix1, Matrix matrix2)
		{
			var matrix = new Matrix();
			for( var i=0; i<matrix.data.Length; i++ )
			{
				matrix.data[i] = matrix1.data[i] + matrix2.data[i];
			}
			return matrix;
		}

        // [ ][ ][ ][ ]   [ ][ ][ ][ ]
        // [ ][ ][ ][ ] x [ ][ ][ ][ ]
        // [ ][ ][ ][ ]   [ ][ ][ ][ ]
        // [ ][ ][ ][ ]   [ ][ ][ ][ ]
        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
        	var matrix = Identity;
			matrix[0,0] = (((matrix1[0,0] * matrix2[0,0]) + (matrix1[0,1] * matrix2[1,0])) + (matrix1[0,2] * matrix2[2,0])) + (matrix1[0,3] * matrix2[3,0]);
			matrix[0,1] = (((matrix1[0,0] * matrix2[0,1]) + (matrix1[0,1] * matrix2[1,1])) + (matrix1[0,2] * matrix2[2,1])) + (matrix1[0,3] * matrix2[3,1]);
			matrix[0,2] = (((matrix1[0,0] * matrix2[0,2]) + (matrix1[0,1] * matrix2[1,2])) + (matrix1[0,2] * matrix2[2,2])) + (matrix1[0,3] * matrix2[3,2]);
			matrix[0,3] = (((matrix1[0,0] * matrix2[0,3]) + (matrix1[0,1] * matrix2[1,3])) + (matrix1[0,2] * matrix2[2,3])) + (matrix1[0,3] * matrix2[3,3]);
			matrix[1,0] = (((matrix1[1,0] * matrix2[0,0]) + (matrix1[1,1] * matrix2[1,0])) + (matrix1[1,2] * matrix2[2,0])) + (matrix1[1,3] * matrix2[3,0]);
			matrix[1,1] = (((matrix1[1,0] * matrix2[0,1]) + (matrix1[1,1] * matrix2[1,1])) + (matrix1[1,2] * matrix2[2,1])) + (matrix1[1,3] * matrix2[3,1]);
			matrix[1,2] = (((matrix1[1,0] * matrix2[0,2]) + (matrix1[1,1] * matrix2[1,2])) + (matrix1[1,2] * matrix2[2,2])) + (matrix1[1,3] * matrix2[3,2]);
			matrix[1,3] = (((matrix1[1,0] * matrix2[0,3]) + (matrix1[1,1] * matrix2[1,3])) + (matrix1[1,2] * matrix2[2,3])) + (matrix1[1,3] * matrix2[3,3]);
			matrix[2,0] = (((matrix1[2,0] * matrix2[0,0]) + (matrix1[2,1] * matrix2[1,0])) + (matrix1[2,2] * matrix2[2,0])) + (matrix1[2,3] * matrix2[3,0]);
			matrix[2,1] = (((matrix1[2,0] * matrix2[0,1]) + (matrix1[2,1] * matrix2[1,1])) + (matrix1[2,2] * matrix2[2,1])) + (matrix1[2,3] * matrix2[3,1]);
			matrix[2,2] = (((matrix1[2,0] * matrix2[0,2]) + (matrix1[2,1] * matrix2[1,2])) + (matrix1[2,2] * matrix2[2,2])) + (matrix1[2,3] * matrix2[3,2]);
			matrix[2,3] = (((matrix1[2,0] * matrix2[0,3]) + (matrix1[2,1] * matrix2[1,3])) + (matrix1[2,2] * matrix2[2,3])) + (matrix1[2,3] * matrix2[3,3]);
			matrix[3,0] = (((matrix1[3,0] * matrix2[0,0]) + (matrix1[3,1] * matrix2[1,0])) + (matrix1[3,2] * matrix2[2,0])) + (matrix1[3,3] * matrix2[3,0]);
			matrix[3,1] = (((matrix1[3,0] * matrix2[0,1]) + (matrix1[3,1] * matrix2[1,1])) + (matrix1[3,2] * matrix2[2,1])) + (matrix1[3,3] * matrix2[3,1]);
			matrix[3,2] = (((matrix1[3,0] * matrix2[0,2]) + (matrix1[3,1] * matrix2[1,2])) + (matrix1[3,2] * matrix2[2,2])) + (matrix1[3,3] * matrix2[3,2]);
			matrix[3,3] = (((matrix1[3,0] * matrix2[0,3]) + (matrix1[3,1] * matrix2[1,3])) + (matrix1[3,2] * matrix2[2,3])) + (matrix1[3,3] * matrix2[3,3]);
        	return matrix;
		}


    	public static Vector operator *(Vector vector, Matrix matrix)
		{
			var returnVector = Vector.Transform(vector, matrix);
			return returnVector;
		}

        public static explicit operator Vector(Matrix matrix)
        {
            return new Vector( matrix[3, 0], matrix[3, 1], matrix[3, 2]) ;
        }


        public static Matrix Identity
        {
			get 
			{
				var identityMatrix = new Matrix();
				SetIdentity(identityMatrix);
				return identityMatrix;
			}
        }

		public static Matrix CreateLookAt(Vector cameraPosition, Vector cameraTarget, Vector cameraUpVector)
		{
			var matrix = Matrix.Identity;
			var vector = Vector.Normalize(cameraPosition - cameraTarget);
			var vector2 = Vector.Normalize(Vector.Cross(cameraUpVector, vector));
			var vector3 = Vector.Cross(vector, vector2);
			matrix[0,0] = vector2.X;
			matrix[0,1] = vector3.X;
			matrix[0,2] = vector.X;
			matrix[0,3] = 0f;
			matrix[1,0] = vector2.Y;
			matrix[1,1] = vector3.Y;
			matrix[1,2] = vector.Y;
			matrix[1,3] = 0f;
			matrix[2,0] = vector2.Z;
			matrix[2,1] = vector3.Z;
			matrix[2,2] = vector.Z;
			matrix[2,3] = 0f;
			matrix[3,0] = -Vector.Dot(vector2, cameraPosition);
			matrix[3,1] = -Vector.Dot(vector3, cameraPosition);
			matrix[3,2] = -Vector.Dot(vector, cameraPosition);
			matrix[3,3] = 1f;
			return matrix;
		}

		public static Matrix CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			Matrix matrix = new Matrix();// Matrix.Identity; 
			var num = 1f / (float)System.Math.Tan(fieldOfView * 0.5f);
			var num9 = num / aspectRatio;
			matrix[0, 0] = num9;
			matrix[0, 1] = matrix[0, 2] = matrix[0, 3] = 0f;
			matrix[1, 1] = num;
			matrix[1, 0] = matrix[1, 2] = matrix[1, 3] = 0f;
			matrix[2, 0] = matrix[2, 1] = 0f;
			matrix[2, 2] = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
			matrix[2, 3] = -1f;
			matrix[3, 0] = matrix[3, 1] = matrix[3, 3] = 0f;
			matrix[3, 2] = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
			return matrix;
		}

		public static Matrix CreateRotationX(float degrees)
		{
			Matrix matrix = new Matrix();

			var num2 = (float)System.Math.Cos(MathHelper.ToRadians(degrees));
			var num = (float)System.Math.Sin(MathHelper.ToRadians(degrees));
			matrix[0, 0] = 1f;
			matrix[0, 1] = 0f;
			matrix[0, 2] = 0f;
			matrix[0, 3] = 0f;
			matrix[1, 0] = 0f;
			matrix[1, 1] = num2;
			matrix[1, 2] = num;
			matrix[1, 3] = 0f;
			matrix[2, 0] = 0f;
			matrix[2, 1] = -num;
			matrix[2, 2] = num2;
			matrix[2, 3] = 0f;
			matrix[3, 0] = 0f;
			matrix[3, 1] = 0f;
			matrix[3, 2] = 0f;
			matrix[3, 3] = 1f;
			return matrix;
		}
 
		public static Matrix CreateRotationY(float degrees)
		{
			var matrix = new Matrix();
			var num2 = (float)System.Math.Cos(MathHelper.ToRadians(degrees));
			var num = (float)System.Math.Sin(MathHelper.ToRadians(degrees));
			matrix[0, 0] = num2;
			matrix[0, 1] = 0f;
			matrix[0, 2] = -num;
			matrix[0, 3] = 0f;
			matrix[1, 0] = 0f;
			matrix[1, 1] = 1f;
			matrix[1, 2] = 0f;
			matrix[1, 3] = 0f;
			matrix[2, 0] = num;
			matrix[2, 1] = 0f;
			matrix[2, 2] = num2;
			matrix[2, 3] = 0f;
			matrix[3, 0] = 0f;
			matrix[3, 1] = 0f;
			matrix[3, 2] = 0f;
			matrix[3, 3] = 1f;
			return matrix;
		}

		public static Matrix CreateRotationZ(float degrees)
		{
			var matrix = new Matrix();
			var num2 = (float)System.Math.Cos(MathHelper.ToRadians(degrees));
			var num = (float)System.Math.Sin(MathHelper.ToRadians(degrees));

			matrix[0, 0] = num2;
			matrix[0, 1] = num;
			matrix[0, 2] = 0f;
			matrix[0, 3] = 0f;
			matrix[1, 0] = -num;
			matrix[1, 1] = num2;
			matrix[1, 2] = 0f;
			matrix[1, 3] = 0f;
			matrix[2, 0] = 0f;
			matrix[2, 1] = 0f;
			matrix[2, 2] = 1f;
			matrix[2, 3] = 0f;
			matrix[3, 0] = 0f;
			matrix[3, 1] = 0f;
			matrix[3, 2] = 0f;
			matrix[3, 3] = 1f;
			return matrix;
		}

		public static Matrix CreateRotation(float xDegrees, float yDegrees, float zDegrees)
		{
			var xRotation = CreateRotationX(xDegrees);
			var yRotation = CreateRotationY(yDegrees);
			var zRotation = CreateRotationZ(zDegrees);

			var matrix = xRotation*yRotation*zRotation;

			return matrix;
			
		}


		public static Matrix CreateTranslation(Vector position)
		{
			var matrix = new Matrix();
			matrix[0, 0] = 1f;
			matrix[0, 1] = 0f;
			matrix[0, 2] = 0f;
			matrix[0, 3] = 0f;
			matrix[1, 0] = 0f;
			matrix[1, 1] = 1f;
			matrix[1, 2] = 0f;
			matrix[1, 3] = 0f;
			matrix[2, 0] = 0f;
			matrix[2, 1] = 0f;
			matrix[2, 2] = 1f;
			matrix[2, 3] = 0f;
			matrix[3, 0] = position.X;
			matrix[3, 1] = position.Y;
			matrix[3, 2] = position.Z;
			matrix[3, 3] = 1f;
			return matrix;
		}

		public static Matrix CreateScale(float scale)
		{
			return CreateScale(new Vector(scale, scale, scale));
		}

		public static Matrix CreateScale(Vector scales)
		{
			var matrix = Matrix.Identity;
			var x = scales.X;
			var y = scales.Y;
			var z = scales.Z;
			matrix[0,0] = x;
			matrix[0,1] = 0f;
			matrix[0,2] = 0f;
			matrix[0,3] = 0f;
			matrix[1,0] = 0f;
			matrix[1,1] = y;
			matrix[1,2] = 0f;
			matrix[1,3] = 0f;
			matrix[2,0] = 0f;
			matrix[2,1] = 0f;
			matrix[2,2] = z;
			matrix[2,3] = 0f;
			matrix[3,0] = 0f;
			matrix[3,1] = 0f;
			matrix[3,2] = 0f;
			matrix[3,3] = 1f;
			return matrix;
		}

		public static Matrix Invert(Matrix matrix)
		{
			var matrix2 = new Matrix();
			var num5 = matrix[0,0];
			var num4 = matrix[0,1];
			var num3 = matrix[0,2];
			var num2 = matrix[0,3];
			var num9 = matrix[1,0];
			var num8 = matrix[1,1];
			var num7 = matrix[1,2];
			var num6 = matrix[1,3];
			var num17 = matrix[2,0];
			var num16 = matrix[2,1];
			var num15 = matrix[2,2];
			var num14 = matrix[2,3];
			var num13 = matrix[3,0];
			var num12 = matrix[3,1];
			var num11 = matrix[3,2];
			var num10 = matrix[3,3];
			var num23 = (num15 * num10) - (num14 * num11);
			var num22 = (num16 * num10) - (num14 * num12);
			var num21 = (num16 * num11) - (num15 * num12);
			var num20 = (num17 * num10) - (num14 * num13);
			var num19 = (num17 * num11) - (num15 * num13);
			var num18 = (num17 * num12) - (num16 * num13);
			var num39 = ((num8 * num23) - (num7 * num22)) + (num6 * num21);
			var num38 = -(((num9 * num23) - (num7 * num20)) + (num6 * num19));
			var num37 = ((num9 * num22) - (num8 * num20)) + (num6 * num18);
			var num36 = -(((num9 * num21) - (num8 * num19)) + (num7 * num18));
			var num = 1f / ((((num5 * num39) + (num4 * num38)) + (num3 * num37)) + (num2 * num36));
			matrix2[0,0] = num39 * num;
			matrix2[1,0] = num38 * num;
			matrix2[2,0] = num37 * num;
			matrix2[3,0] = num36 * num;
			matrix2[0,1] = -(((num4 * num23) - (num3 * num22)) + (num2 * num21)) * num;
			matrix2[1,1] = (((num5 * num23) - (num3 * num20)) + (num2 * num19)) * num;
			matrix2[2,1] = -(((num5 * num22) - (num4 * num20)) + (num2 * num18)) * num;
			matrix2[3,1] = (((num5 * num21) - (num4 * num19)) + (num3 * num18)) * num;
			var num35 = (num7 * num10) - (num6 * num11);
			var num34 = (num8 * num10) - (num6 * num12);
			var num33 = (num8 * num11) - (num7 * num12);
			var num32 = (num9 * num10) - (num6 * num13);
			var num31 = (num9 * num11) - (num7 * num13);
			var num30 = (num9 * num12) - (num8 * num13);
			matrix2[0,2] = (((num4 * num35) - (num3 * num34)) + (num2 * num33)) * num;
			matrix2[1,2] = -(((num5 * num35) - (num3 * num32)) + (num2 * num31)) * num;
			matrix2[2,2] = (((num5 * num34) - (num4 * num32)) + (num2 * num30)) * num;
			matrix2[3,2] = -(((num5 * num33) - (num4 * num31)) + (num3 * num30)) * num;
			var num29 = (num7 * num14) - (num6 * num15);
			var num28 = (num8 * num14) - (num6 * num16);
			var num27 = (num8 * num15) - (num7 * num16);
			var num26 = (num9 * num14) - (num6 * num17);
			var num25 = (num9 * num15) - (num7 * num17);
			var num24 = (num9 * num16) - (num8 * num17);
			matrix2[0,3] = -(((num4 * num29) - (num3 * num28)) + (num2 * num27)) * num;
			matrix2[1,3] = (((num5 * num29) - (num3 * num26)) + (num2 * num25)) * num;
			matrix2[2,3] = -(((num5 * num28) - (num4 * num26)) + (num2 * num24)) * num;
			matrix2[3,3] = (((num5 * num27) - (num4 * num25)) + (num3 * num24)) * num;
			return matrix2;
		}

		public override string ToString()
		{
			var format = "[ {0:##.##} - {1:##.##} - {2:##.##} - {3:##.##} ])";
			var row1 = string.Format(format,
										this[0, 0], this[0, 1], this[0, 2], this[0, 3]
				);
			var row2 = string.Format(format,
										this[1, 0], this[1, 1], this[1, 2], this[1, 3]
				);
			var row3 = string.Format(format,
										this[2, 0], this[2, 1], this[2, 2], this[2, 3]
				);
			var row4 = string.Format(format,
										this[3, 0], this[3, 1], this[3, 2], this[3, 3]
				);

			return string.Format("{0}\n{1}\n{2}\n{3}\n",
			                     row1,
			                     row2,
			                     row3,
			                     row4);
		}
 

	}
}
