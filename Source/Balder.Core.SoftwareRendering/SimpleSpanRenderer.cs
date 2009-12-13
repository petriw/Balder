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
using Balder.Core.Imaging;

namespace Balder.Core.SoftwareRendering
{
	public class SimpleSpanRenderer : ISpanRenderer
	{
		public const int DefaultSpanLength = 640;

		private static readonly Interpolator DepthInterpolator;
		private static readonly Interpolator GouraudInterpolator;
		private static readonly Interpolator TextureInterpolator;
		private static Color WorkingColor;

		static SimpleSpanRenderer()
		{
			DepthInterpolator = new Interpolator();
			DepthInterpolator.SetNumberOfInterpolationPoints(1);
			DepthInterpolator.InitializePoints(DefaultSpanLength);

			GouraudInterpolator = new Interpolator();
			GouraudInterpolator.SetNumberOfInterpolationPoints(5);
			GouraudInterpolator.InitializePoints(DefaultSpanLength);

			TextureInterpolator = new Interpolator();
			TextureInterpolator.SetNumberOfInterpolationPoints(3);
			TextureInterpolator.InitializePoints(DefaultSpanLength);
		}

		public bool SupportsDepthBuffer { get { return true; } }

		public void Flat(IBuffers buffer, Span span, Color color)
		{
			var spreadCount = span.Length; //span.XEnd - span.XStart;
			DepthInterpolator.SetPoint(0, span.ZStart, span.ZEnd);
			var yOffset = span.Y*buffer.FrameBuffer.Stride;
			var rOffset = buffer.FrameBuffer.RedPosition;
			var gOffset = buffer.FrameBuffer.GreenPosition;
			var bOffset = buffer.FrameBuffer.BluePosition;
			var aOffset = buffer.FrameBuffer.AlphaPosition;
			var bufferOffset = yOffset + span.XStart;
			var depthBufferOffset = (buffer.Width*span.Y) + span.XStart;
			DepthInterpolator.Interpolate(spreadCount);

			var colorAsInt = (int)color.ToUInt32();

			var xOffset = span.XStart;
			for( var index=0; index<spreadCount; index++ )
			{
				if (xOffset >= 0 && xOffset < buffer.Width)
				{
					var z = DepthInterpolator.Points[0].InterpolatedValues[index];
					var bufferZ = (UInt32) ((1.0f-z)*(float) UInt32.MaxValue);
					
					if (bufferZ > buffer.DepthBuffer[depthBufferOffset] &&
					    z >= 0f &&
					    z < 1f
						)
					{
						buffer.FrameBuffer.Pixels[bufferOffset] = colorAsInt;
						buffer.DepthBuffer[depthBufferOffset] = bufferZ;
					}
				}

				xOffset++;
				bufferOffset++;
				depthBufferOffset++;
			}
		}

		public void Gouraud(IBuffers buffer, Span span)
		{
			var spreadCount = span.XEnd - span.XStart;
			GouraudInterpolator.SetPoint(0, span.ZStart, span.ZEnd);
			GouraudInterpolator.SetPoint(1, span.ColorStart.RedAsFloat, span.ColorEnd.RedAsFloat);
			GouraudInterpolator.SetPoint(2, span.ColorStart.GreenAsFloat, span.ColorEnd.GreenAsFloat);
			GouraudInterpolator.SetPoint(3, span.ColorStart.BlueAsFloat, span.ColorEnd.BlueAsFloat);
			GouraudInterpolator.SetPoint(4, span.ColorStart.AlphaAsFloat, span.ColorEnd.AlphaAsFloat);

			var yOffset = buffer.FrameBuffer.Stride * span.Y;
			var rOffset = buffer.FrameBuffer.RedPosition;
			var gOffset = buffer.FrameBuffer.GreenPosition;
			var bOffset = buffer.FrameBuffer.BluePosition;
			var aOffset = buffer.FrameBuffer.AlphaPosition;
			var bufferOffset = yOffset + span.XStart;
			var depthBufferOffset = (buffer.Width*span.Y)+span.XStart;
			GouraudInterpolator.Interpolate(spreadCount);

			var xOffset = span.XStart;

			for (var index = 0; index < spreadCount; index++)
			{
				if (xOffset >= 0 && xOffset < buffer.Width)
				{

					var z = GouraudInterpolator.Points[0].InterpolatedValues[index];
					var bufferZ = (UInt32) ((1.0f-z)*(float) UInt32.MaxValue);

					if (bufferZ > buffer.DepthBuffer[depthBufferOffset] &&
					    z >= 0f &&
					    z < 1f
						)
					{
						buffer.DepthBuffer[depthBufferOffset] = bufferZ;

						WorkingColor.RedAsFloat = GouraudInterpolator.Points[1].InterpolatedValues[index];
						WorkingColor.GreenAsFloat = GouraudInterpolator.Points[2].InterpolatedValues[index];
						WorkingColor.BlueAsFloat = GouraudInterpolator.Points[3].InterpolatedValues[index];
						WorkingColor.AlphaAsFloat = GouraudInterpolator.Points[4].InterpolatedValues[index];
						WorkingColor.Clamp();
						var color = (int)WorkingColor.ToUInt32();
						buffer.FrameBuffer.Pixels[bufferOffset] = color;
					}
				}

				xOffset++;
				bufferOffset ++;

				depthBufferOffset++;
			}
		}

		public void Texture(IBuffers buffer, Span span, Image image, ImageContext texture)
		{
			var spreadCount = span.XEnd - span.XStart;
			TextureInterpolator.SetPoint(0, span.ZStart, span.ZEnd);
			TextureInterpolator.SetPoint(1, span.UStart, span.UEnd);
			TextureInterpolator.SetPoint(2, span.VStart, span.VEnd);
			var yOffset = span.Y * buffer.FrameBuffer.Stride;
			var rOffset = buffer.FrameBuffer.RedPosition;
			var gOffset = buffer.FrameBuffer.GreenPosition;
			var bOffset = buffer.FrameBuffer.BluePosition;
			var aOffset = buffer.FrameBuffer.AlphaPosition;
			var bufferOffset = yOffset + span.XStart;
			var depthBufferOffset = (buffer.Width * span.Y) + span.XStart;
			TextureInterpolator.Interpolate(spreadCount);

			var xOffset = span.XStart;

			for (var index = 0; index < spreadCount; index++)
			{
				if (xOffset >= 0 && xOffset < buffer.Width)
				{
					var z = TextureInterpolator.Points[0].InterpolatedValues[index];
					var bufferZ = (UInt32) ((1.0f - z)*(float) UInt32.MaxValue);


					var u = TextureInterpolator.Points[1].InterpolatedValues[index];
					var v = TextureInterpolator.Points[2].InterpolatedValues[index];

					var intu = (int) (u*image.Width) & (image.Width - 1);
					var intv = (int) (v*image.Height) & (image.Height - 1);

					var texel = ((intv*image.Width) + intu);
					
					if (bufferZ > buffer.DepthBuffer[depthBufferOffset] &&
					    z >= 0f &&
					    z < 1f
						)
					{
						buffer.FrameBuffer.Pixels[bufferOffset] = texture.Pixels[texel];

						buffer.DepthBuffer[depthBufferOffset] = bufferZ;
					}
				}

				bufferOffset ++;
				depthBufferOffset++;
				xOffset++;
			}
		}
	}
}
