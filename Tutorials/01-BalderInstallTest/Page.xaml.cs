using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Balder;
using Balder.Diagnostics;
using Balder.Math;
using Balder.Objects;
using Application=Balder.Application;
using System.Windows.Media.Imaging;
using Matrix = System.Windows.Media.Matrix;

namespace BalderTestApplication
{
	public partial class Page : UserControl
	{
		public Page()
		{
			InitializeComponent();

			this.cubeStoryboard.Begin();
			//this.cubeXStoryboard.Begin();
			this.Loaded += new RoutedEventHandler(Page_Loaded);
			
		}

		void Page_Loaded(object sender, RoutedEventArgs e)
		{
		}

	}
}
