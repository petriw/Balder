using System;
using Balder.Core.Content;
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
				//ActualNode = ContentManager.Load<Core.Objects.Geometries.Mesh>(AssetName.ToString());
			}
		}
	}
}
