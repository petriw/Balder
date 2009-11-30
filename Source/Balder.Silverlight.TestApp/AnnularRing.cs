using Balder.Core;
using Balder.Core.Content;
using Balder.Core.Display;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;


namespace Balder.Silverlight.TestApp
{
	public class AnnularRing : RenderableNode
	{
		private Geometry _geometry;

		public AnnularRing(IContentManager contentManager, float innerRadius, float outerRadius, int slices)
		{
			ContentManager = contentManager;
			_geometry = ContentManager.CreateAssetPart<Geometry>();
			InnerRadius = innerRadius;
			OuterRadius = outerRadius;
			Slices = slices;

			PrepareVertices();
			PrepareFaces();
		}

		public float InnerRadius { get; private set; }
		public float OuterRadius { get; private set; }
		public int Slices { get; private set; }
		public IContentManager ContentManager { get; private set; }

		private void PrepareVertices()
		{
			Vertex[] vertices = new Vertex[2 * Slices];

			_geometry.GeometryContext.AllocateVertices(2 * Slices);

			for (int i = 0; i < Slices; i++)
			{
				float innerX = (float)(InnerRadius * System.Math.Sin(2 * i * System.Math.PI / Slices));
				float innerY = (float)(InnerRadius * System.Math.Cos(2 * i * System.Math.PI / Slices));

				_geometry.GeometryContext.SetVertex(i, new Vertex(innerX, innerY, 0));

				float outerX = (float)(InnerRadius * System.Math.Sin(2 * i * System.Math.PI / Slices));
				float outerY = (float)(InnerRadius * System.Math.Cos(2 * i * System.Math.PI / Slices));
				_geometry.GeometryContext.SetVertex(i + Slices, new Vertex(innerX, innerY, 0));
			}
		}

		private void PrepareFaces()
		{
			_geometry.GeometryContext.AllocateFaces(2 * Slices);

			int offset = 0;
			for (int slice = 0; slice < Slices; slice++)
			{
				if (Slices + slice + 1 < 2 * Slices)
				{
					_geometry.GeometryContext.SetFace(offset, new Face(slice, Slices + slice, Slices + slice + 1));
					_geometry.GeometryContext.SetFace(offset + 1, new Face(Slices + slice + 1, slice + 1, slice));
				}
				else
				{
					_geometry.GeometryContext.SetFace(offset, new Face(slice, Slices + slice, 0));
					_geometry.GeometryContext.SetFace(offset + 1, new Face(0, slice + 1, slice));
				}
				offset += 2;
			}
		}

		public override void Render(Viewport viewport, Matrix view, Matrix projection)
		{
			_geometry.GeometryContext.Render(viewport,this, view,projection,World);
		}
	}
}
