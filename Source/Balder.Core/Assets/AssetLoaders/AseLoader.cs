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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Balder.Core.Content;
using Balder.Core.Exceptions;
using Balder.Core.Imaging;
using Balder.Core.Materials;
using Balder.Core.Objects.Geometries;
using Balder.Core.ReadableRex;
using Balder.Core.ReadableRex.LinqToRegex;


namespace Balder.Core.Assets.AssetLoaders
{
	public class AseLoader : AssetLoader<Geometry>
	{
		private readonly IAssetLoaderService _assetLoaderService;


		public AseLoader(IAssetLoaderService assetLoaderService, IFileLoader fileLoader, IContentManager contentManager)
			: base(fileLoader, contentManager)
		{
			_assetLoaderService = assetLoaderService;
		}


		public override Geometry[] Load(string assetName)
		{
			var stream = FileLoader.GetStream(assetName);
			if( null == stream )
			{
				throw new AssetNotFoundException(assetName);
			}
			var streamReader = new StreamReader(stream);
			var file = streamReader.ReadToEnd();

			var format = CultureInfo.InvariantCulture.NumberFormat;

			var geometry = ContentManager.CreateAssetPart<Geometry>();
			var context = geometry.GeometryContext;

			var materials = LoadMaterials(assetName, context, format, file);
			LoadVertices(context, format, file);
			LoadFaces(context, format, file, materials);
			LoadTextureCoordinates(context, format, file);
			LoadTextureFaces(context, format, file);
			GeometryHelper.CalculateVertexNormals(context);

			return new[] { geometry };
		}

		private Material[] LoadMaterials(string aseAssetName, IGeometryContext context, IFormatProvider format, string data)
		{
			var materials = new List<Material>();

			var rootPath = Path.GetDirectoryName(aseAssetName);

			var query = from match in RegexQuery.Against(data)
			            where	match
			            	.RegEx("\t")
			            	.Literal("*MATERIAL")
			            	.WhiteSpace
			            	.Group(Pattern.With.Digit.Repeat.ZeroOrMore)
			            	.WhiteSpace
			            	.Literal("{")
			            	.RegEx("[\n\r]*")
			            	.IsTrue()
			            select match;
			
			foreach( var match in query )
			{
				var materialEnd = data.IndexOf("\n\t}", match.Index);
				if( materialEnd > 0 )
				{
					materialEnd += 2;
					var material = new Material();
					materials.Add(material);
					var materialString = data.Substring(match.Index, materialEnd - match.Index);
					var diffuseIndex = materialString.IndexOf("\t\t*MAP_DIFFUSE");
					if( diffuseIndex > 0 )
					{
						var diffuseEnd = materialString.IndexOf("\n\t\t}", diffuseIndex);
						if( diffuseEnd > 0 )
						{
							var diffuseString = materialString.Substring(diffuseIndex, diffuseEnd - diffuseIndex);
							var bitmapIndex = diffuseString.IndexOf("\t\t\t*BITMAP");
							var eolIndex = diffuseString.IndexOf('\n', bitmapIndex);
							if( eolIndex > 0 )
							{
								var bitmapString = diffuseString.Substring(bitmapIndex, eolIndex - bitmapIndex);
								var fileIndex = bitmapString.IndexOf('"');
								if( fileIndex > 0 )
								{
									var file = bitmapString.Substring(fileIndex + 1).Replace('"', ' ').Trim();
									var relativeFile = Path.GetFileName(file);
									var actualFile = string.IsNullOrEmpty(rootPath)
									                 	? relativeFile
									                 	: string.Format("{0}//{1}", rootPath, relativeFile);
									var loader = _assetLoaderService.GetLoader<Image>(actualFile);
									var frames = loader.Load(actualFile);
									material.DiffuseMap = frames[0];
								}
							}
						}
					}
				}
			}
			return materials.ToArray();

			/*
			var materialRegex = new Regex(@"\t\*MATERIAL\s*(\d*)\s*{[\n\r](.*)\t}");
			var matches = materialRegex.Matches(data);

			var assetName = "BlueEnvMap.png";
			var loader = AssetLoaderManager.Instance.GetLoader<Image>(assetName);
			var frames = loader.Load(assetName);

			var material = new Material
							{
								DiffuseMap = frames[0]
							};

			for (var faceIndex = 0; faceIndex < context.FaceCount; faceIndex++)
			{
				context.SetMaterial(faceIndex, material);
			}
			 */
		}

		private static void LoadVertices(IGeometryContext context, IFormatProvider format, string data)
		{
			var meshRegex = new Regex(@"\t\t\t\*MESH_VERTEX\s*(\d*)\t([\-\d\.]*)\t([\-\d\.]*)\t([\-\d\.]*)[\n\r]*");
			var matches = meshRegex.Matches(data);

			context.AllocateVertices(matches.Count);

			var vertexIndex = 0;
			foreach (Match match in matches)
			{
				var x = float.Parse(match.Groups[2].Value, format);
				var z = float.Parse(match.Groups[3].Value, format);
				var y = float.Parse(match.Groups[4].Value, format);

				context.SetVertex(vertexIndex, new Vertex(x, y, z));
				vertexIndex++;
			}
		}

		private static void LoadFaces(IGeometryContext context, IFormatProvider format, string data, Material[] materials)
		{
			var faceRegex = new Regex(@"\t\t\t\*MESH_FACE\s*[0-9]*:\s*A:\s*([0-9]*)\s*B:\s*([0-9]*)\s*C:\s*([0-9]*)\s*AB:\s*([0-9]*)\s*BC:\s*([0-9]*)\s*CA:\s*([0-9]*)\t\s*\*MESH_SMOOTHING\s*[0-9]*\s*\t\*MESH_MTLID\s*([0-9]*)[\n\r]*");
			var matches = faceRegex.Matches(data);

			context.AllocateFaces(matches.Count);

			var faceIndex = 0;
			foreach (Match match in matches)
			{
				var a = Convert.ToInt32(match.Groups[1].Value);
				var b = Convert.ToInt32(match.Groups[2].Value);
				var c = Convert.ToInt32(match.Groups[3].Value);
				var face = new Face { A = a, B = b, C = c };

				/*
				var materialIndex = Convert.ToInt32(match.Groups[4].Value);
				if( materialIndex < materials.Length )
				{
					face.Material = materials[materialIndex];
				}
				 * */
				
				if( materials.Length > 0 )
				{
					face.Material = materials[0];
				}


				context.SetFace(faceIndex, face);
				faceIndex++;
			}
		}

		private static void LoadTextureCoordinates(IGeometryContext context, IFormatProvider format, string data)
		{
			var meshRegex = new Regex(@"\t\t\t\*MESH_TVERT\s*(\d*)\t([\-\d\.]*)\t([\-\d\.]*)\t([\-\d\.]*)[\n\r]*");
			var matches = meshRegex.Matches(data);

			context.AllocateTextureCoordinates(matches.Count);

			var vertexIndex = 0;
			foreach (Match match in matches)
			{
				var u = float.Parse(match.Groups[2].Value, format);
				var v = float.Parse(match.Groups[3].Value, format);
				v = 1f - v;
				//var y = -float.Parse(match.Groups[4].Value, format);

				context.SetTextureCoordinate(vertexIndex, new TextureCoordinate(u, v));
				vertexIndex++;
			}
		}

		private static void LoadTextureFaces(IGeometryContext context, IFormatProvider format, string data)
		{
			// *MESH_TFACE 0	0	5	6
			var faceRegex = new Regex(@"\t\t\t\*MESH_TFACE\s*(\d*)\t([0-9]*)\t([0-9]*)\t([0-9]*)[\n\r]*");
			var matches = faceRegex.Matches(data);

			foreach (Match match in matches)
			{
				var faceIndex = Convert.ToInt32(match.Groups[1].Value);
				var a = Convert.ToInt32(match.Groups[2].Value);
				var b = Convert.ToInt32(match.Groups[3].Value);
				var c = Convert.ToInt32(match.Groups[4].Value);

				context.SetFaceTextureCoordinateIndex(faceIndex, a, b, c);
			}
		}

		public override string[] FileExtensions
		{
			get { return new[] { "ase" }; }
		}
	}
}