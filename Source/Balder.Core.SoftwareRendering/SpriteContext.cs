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
using Balder.Core.Display;
using Balder.Core.Imaging;
using Balder.Core.Math;
using Balder.Core.Objects.Flat;

namespace Balder.Core.SoftwareRendering
{
	public class SpriteContext : ISpriteContext
	{
		private static readonly Interpolator XScalingInterpolator;
		private static readonly Interpolator YScalingInterpolator;

		static SpriteContext()
		{
			XScalingInterpolator = new Interpolator();
			XScalingInterpolator.SetNumberOfInterpolationPoints(1);
			YScalingInterpolator = new Interpolator();
			YScalingInterpolator.SetNumberOfInterpolationPoints(1);
		}

		public void Render(Viewport viewport, Sprite sprite, Matrix view, Matrix projection, Matrix world, float xScale, float yScale, float rotation)
		{
			var buffer = BufferManager.Instance.Current;

			var image = sprite.CurrentFrame;

			
			var position = new Vector(0, 0, 0);
			var transformedPosition = Vector.Transform(position, world, view);
			var translatedPosition = Vector.Translate(transformedPosition, projection, viewport.Width, viewport.Height);
			var depthBufferAdjustedZ = -transformedPosition.Z / viewport.Camera.DepthDivisor;
			var positionOffset = (((int)translatedPosition.X)) + (((int)translatedPosition.Y) * buffer.FrameBuffer.Stride);

			var bufferSize = buffer.FrameBuffer.Stride*buffer.Height;
			var bufferZ = (UInt32)(depthBufferAdjustedZ * (float)UInt32.MaxValue);
			if( depthBufferAdjustedZ < 0f || depthBufferAdjustedZ >= 1f)
			{
				return;
			}
			
			if( xScale != 1f || yScale != 1f )
			{
				RenderScaled(buffer, positionOffset, sprite.CurrentFrame, translatedPosition, bufferSize, bufferZ, xScale, yScale);
				
			} else
			{
				RenderUnscaled(buffer,positionOffset,sprite.CurrentFrame,translatedPosition,bufferSize,bufferZ);
			}
		}

		private static void RenderUnscaled(IBuffers buffer, int positionOffset, Image image, Vector translatedPosition, int bufferSize, UInt32 bufferZ)
		{
			var rOffset = buffer.FrameBuffer.RedPosition;
			var gOffset = buffer.FrameBuffer.GreenPosition;
			var bOffset = buffer.FrameBuffer.BluePosition;
			var aOffset = buffer.FrameBuffer.AlphaPosition;
			var imageContext = image.ImageContext as ImageContext;
			var spriteOffset = 0;

			for (var y = 0; y < image.Height; y++)
			{
				var offset = y * buffer.FrameBuffer.Stride;
				var depthBufferOffset = (buffer.Width * ((int)translatedPosition.Y + y)) + (int)translatedPosition.X;
				for (var x = 0; x < image.Width; x++)
				{
					var actualOffset = offset + positionOffset;

					if (actualOffset >= 0 && actualOffset < bufferSize &&
						bufferZ < buffer.DepthBuffer[depthBufferOffset])
					{
						buffer.FrameBuffer.Pixels[actualOffset] = imageContext.Pixels[spriteOffset];
						buffer.DepthBuffer[depthBufferOffset] = bufferZ;
					}
					offset ++;
					spriteOffset ++;
					depthBufferOffset++;
				}
			}
		}

		private static void RenderScaled(IBuffers buffer, int positionOffset, Image image, Vector translatedPosition, int bufferSize, UInt32 bufferZ, float xScale, float yScale)
		{
			var rOffset = buffer.FrameBuffer.RedPosition;
			var gOffset = buffer.FrameBuffer.GreenPosition;
			var bOffset = buffer.FrameBuffer.BluePosition;
			var aOffset = buffer.FrameBuffer.AlphaPosition;
			var imageContext = image.ImageContext as ImageContext;

			var actualWidth = (int) (((float) image.Width)*xScale);
			var actualHeight = (int) (((float) image.Height)*yScale);

			if( actualWidth <= 0 || actualHeight <=0 )
			{
				return;
			}

			var spriteOffset = 0;

			XScalingInterpolator.SetPoint(0,0f,image.Width);
			XScalingInterpolator.Interpolate(actualWidth);

			YScalingInterpolator.SetPoint(0,0f,image.Height);
			YScalingInterpolator.Interpolate(actualHeight);
			

			for (var y = 0; y < actualHeight; y++)
			{
				var offset = y*buffer.FrameBuffer.Stride;
				var depthBufferOffset = (buffer.Width*((int) translatedPosition.Y + y)) + (int) translatedPosition.X;

				var spriteY = (int)YScalingInterpolator.Points[0].InterpolatedValues[y];

				for (var x = 0; x < actualWidth; x++)
				{
					var actualOffset = offset + positionOffset;
					
					var spriteX = (int)XScalingInterpolator.Points[0].InterpolatedValues[x];
					spriteOffset = (int)((spriteY*image.Width) + spriteX);

					if (actualOffset >= 0 && actualOffset < bufferSize &&
					    bufferZ < buffer.DepthBuffer[depthBufferOffset])
					{
						buffer.FrameBuffer.Pixels[actualOffset] = imageContext.Pixels[spriteOffset];
						buffer.DepthBuffer[depthBufferOffset] = bufferZ;
					}
					offset ++;
					
					depthBufferOffset++;
				}
			}
		}
	}
}
