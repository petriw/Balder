using System;
using Balder.Core.Helpers;

namespace Balder.Core.Objects.Geometries
{
	public partial class Mesh
	{
		public static DependencyProperty<Mesh, Uri> AssetNameProperty =
			DependencyProperty<Mesh, Uri>.Register(o => o.AssetName);
		public Uri AssetName
		{
			get { return AssetNameProperty.GetValue(this); }
			set { AssetNameProperty.SetValue(this, value); }
		}

		protected override void OnLoaded()
		{
			if (null != AssetName)
			{
				Load(AssetName.ToString());
			}
		}
	}
}
