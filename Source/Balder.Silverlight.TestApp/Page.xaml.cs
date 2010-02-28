using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Balder.Silverlight.TestApp
{
	public partial class Page : UserControl
	{
		public Page()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var storyboard = Resources["_testStoryboard"] as Storyboard;
			storyboard.Begin();

		}
	}
}
