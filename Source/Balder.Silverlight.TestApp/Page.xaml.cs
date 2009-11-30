using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Balder.Core;
using Color=Balder.Core.Color;

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
			_cameraStoryboard.Begin();
		}

		private void _teapot_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_teapot.Color = Colors.Brown;
		}

		private void _teapot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_teapot.Color = Colors.Blue;
		}

		private void _teapot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_teapot.Color = Colors.Red;

		}

		private void _teapot_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_teapot.Color = Colors.Brown;
		}
	}
}
