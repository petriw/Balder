using System.Windows.Media;
using Balder.Core.Helpers;

namespace Balder.Silverlight.Controls
{
	public class OmniLight : Light
	{
		public Core.Lighting.OmniLight ActualLight { get { return ActualNode as Core.Lighting.OmniLight; } }

		public OmniLight()
		{
			var light = new Core.Lighting.OmniLight();
			ActualNode = light;
		}

		public static DependencyProperty<OmniLight, Color> DiffuseProperty =
			DependencyProperty<OmniLight, Color>.Register(o => o.Diffuse);
		public Color Diffuse
		{
			get { return DiffuseProperty.GetValue(this); }
			set
			{
				DiffuseProperty.SetValue(this, value);
				ActualLight.Diffuse = Core.Color.FromSystemColor(value);
			}
		}

		public static DependencyProperty<OmniLight, Color> AmbientProperty =
			DependencyProperty<OmniLight, Color>.Register(o => o.Ambient);
		public Color Ambient
		{
			get { return AmbientProperty.GetValue(this); }
			set
			{
				AmbientProperty.SetValue(this, value);
				ActualLight.Ambient = Core.Color.FromSystemColor(value);
			}
		}

		public static DependencyProperty<OmniLight, Color> SpecularProperty =
			DependencyProperty<OmniLight, Color>.Register(o => o.Specular);
		public Color Specular
		{
			get { return SpecularProperty.GetValue(this); }
			set
			{
				SpecularProperty.SetValue(this, value);
				ActualLight.Specular = Core.Color.FromSystemColor(value);
			}
		}

		public static DependencyProperty<OmniLight, double> StrengthProperty =
		DependencyProperty<OmniLight, double>.Register(o => o.Strength);
		public double Strength
		{
			get { return StrengthProperty.GetValue(this); }
			set
			{
				StrengthProperty.SetValue(this, value);
				ActualLight.Strength = (float) value;
			}
		}


		public static DependencyProperty<OmniLight, double> RangeProperty =
		DependencyProperty<OmniLight, double>.Register(o => o.Range);
		public double Range
		{
			get { return RangeProperty.GetValue(this); }
			set
			{
				RangeProperty.SetValue(this, value);
				ActualLight.Range = (float) value;
			}
		}
	}
}
