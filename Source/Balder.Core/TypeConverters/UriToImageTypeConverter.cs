using System;
using System.ComponentModel;
using System.Globalization;
using Balder.Core.Assets;
using Balder.Core.Imaging;
using Ninject.Core;

namespace Balder.Core.TypeConverters
{
	public class UriToImageTypeConverter : TypeConverter
	{
		public UriToImageTypeConverter()
		{
			Runtime.Instance.WireUpDependencies(this);
		}

		[Inject]
		public IAssetLoaderService AssetLoaderService { get; set; }


		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType.Equals(typeof(string));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if( value is string )
			{
				var uri = value as string;
				var loader = AssetLoaderService.GetLoader<Image>(uri);
				var images = loader.Load(uri);
				if( images.Length == 1 )
				{
					return images[0];
				}
			}
			return null;
		}
	}
}
