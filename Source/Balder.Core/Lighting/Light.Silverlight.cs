using System.ComponentModel;
using Balder.Core.Execution;
using Balder.Core.TypeConverters;

namespace Balder.Core.Lighting
{
	public partial class Light
	{
		public static readonly Property<Light, Color> DiffuseProp = Property<Light, Color>.Register(l => l.Diffuse);

		[TypeConverter(typeof(ColorConverter))]
		public Color Diffuse
		{
			get { return DiffuseProp.GetValue(this); }
			set { DiffuseProp.SetValue(this, value); }
		}

		public static readonly Property<Light, Color> SpecularProp = Property<Light, Color>.Register(l => l.Specular);

		[TypeConverter(typeof(ColorConverter))]
		public Color Specular
		{
			get { return SpecularProp.GetValue(this); }
			set { SpecularProp.SetValue(this, value); }
		}

		public static readonly Property<Light, Color> AmbientProp = Property<Light, Color>.Register(l => l.Ambient);

		[TypeConverter(typeof(ColorConverter))]
		public Color Ambient
		{
			get { return AmbientProp.GetValue(this); }
			set { AmbientProp.SetValue(this, value); }
		}
	}
}
