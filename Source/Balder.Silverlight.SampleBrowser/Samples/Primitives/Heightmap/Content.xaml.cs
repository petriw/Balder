using System;
using System.ComponentModel;
using Balder.Core;
using Balder.Core.Objects.Geometries;
using Balder.Silverlight.SampleBrowser.Constants;
using Balder.Silverlight.SampleBrowser.Content;

namespace Balder.Silverlight.SampleBrowser.Samples.Primitives.Heightmap
{
	[Category(Categories.Primitives)]
	[SamplePage("Heightmap")]
	[Description("Sample showing how to use the Heightmap primitive")]
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();
		}

		private double _sin;
		private double _movement;

		private void Heightmap_HeightInput(object sender, HeightmapEventArgs e)
		{
			var height = Math.Sin(_sin + _movement)*2;

			e.Height = (float)height;
			var highlight = (byte)((height*16f)+32f);

			e.Color = Color.FromArgb(0xff, highlight, highlight, highlight);

			_sin += 0.03;

			if( e.GridX == HeightMap.LengthSegments-1 && 
			    e.GridY == HeightMap.HeightSegments-1 )
			{
				_sin = 0;
				_movement += 0.05;
			}

		}
	}
}