using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Balder.Core;

namespace Balder.Silverlight.TestApp
{
	public partial class Page : UserControl
	{
		public Page()
		{
			InitializeComponent();

			Loaded += Page_Loaded;



		}

		void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			int i = 0;
			i++;

			
			
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
//			_cameraStoryboard.Begin();
		}
	}
}
