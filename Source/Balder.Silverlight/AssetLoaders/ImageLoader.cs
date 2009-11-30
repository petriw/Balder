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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Balder.Core.Assets;
using Balder.Core.Content;
using Balder.Core.Imaging;

namespace Balder.Silverlight.AssetLoaders
{
	public class ImageLoader : AssetLoader<Image>
	{
		public ImageLoader(IFileLoader fileLoader, IContentManager contentManager)
			: base(fileLoader, contentManager)
		{
		}


		public override Image[] Load(string assetName)
		{
			var stream = FileLoader.GetStream(assetName);

			var bitmapImage = new BitmapImage();
			bitmapImage.SetSource(stream);


			var writeableBitmap = new WriteableBitmap(bitmapImage);
			var width = writeableBitmap.PixelWidth;
			var height = writeableBitmap.PixelHeight;
			var frame = ContentManager.CreateAssetPart<Image>();
			frame.Width = width;
			frame.Height = height;

			var imageAsBytes = new byte[width * height * 4];

			Buffer.BlockCopy(writeableBitmap.Pixels,0,imageAsBytes,0,imageAsBytes.Length);

			frame.ImageContext.SetFrame(imageAsBytes);

			return new[] { frame };
		}


		public override string[] FileExtensions
		{
			get { return new[] { "png", "jpg" }; }
		}
	}
}
