using System;
using Balder.Core.Helpers;

namespace Balder.Core.Objects.Flat
{
	public partial class Sprite
	{
		public static DependencyProperty<Sprite, Uri> AssetNameProperty =
			DependencyProperty<Sprite, Uri>.Register(o => o.AssetName);
		public Uri AssetName
		{
			get { return AssetNameProperty.GetValue(this); }
			set { AssetNameProperty.SetValue(this, value); }
		}

		protected override void Prepare()
		{
			if (null != AssetName)
			{
				Load(AssetName.ToString());
			}
			base.Prepare();
		}
	}
}
