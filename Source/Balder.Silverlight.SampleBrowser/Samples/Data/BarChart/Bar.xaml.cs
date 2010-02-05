using Balder.Core.Helpers;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.BarChart
{
	public partial class Bar
	{
		public Bar()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty<Bar, double> ValueProperty =
			DependencyProperty<Bar, double>.Register(b => b.Value);
		public double Value
		{
			get { return ValueProperty.GetValue(this); }
			set
			{
				ValueProperty.SetValue(this,value);
				Scale.Y = value/10;
			}
		}
	}
}
