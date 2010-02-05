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
using Balder.Core.Display;
using Balder.Core.Materials;
using Balder.Core.Math;

namespace Balder.Core.Objects.Geometries
{
	public interface IGeometryContext
	{
		int FaceCount { get; }
		int VertexCount { get; }
		int TextureCoordinateCount { get; }
		int LineCount { get; }

		void AllocateFaces(int count);
		void SetFace(int index,Face face);
		Face[] GetFaces();

		void AllocateVertices(int count);
		void SetVertex(int index,Vertex vertex);
		Vertex[] GetVertices();

		void AllocateLines(int count);
		void SetLine(int index, Line line);
		Line[] GetLines();
		
		void AllocateTextureCoordinates(int count);
		void SetTextureCoordinate(int index,TextureCoordinate textureCoordinate);
		void SetFaceTextureCoordinateIndex(int index, int a, int b, int c);

		void SetMaterial(int index, Material material);

		void SetMaterialForAllFaces(Material material);

		void Render(Viewport viewport, RenderableNode geometry, Matrix view, Matrix projection, Matrix world);
	}
}