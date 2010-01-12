using System;
using System.Windows;
using Balder.Core.Helpers;

namespace Balder.Silverlight.SampleBrowser.Features.Resources
{
	public partial class View
	{
		public View()
		{
			InitializeComponent();

			Loaded += ViewLoaded;
		}

		private void ViewLoaded(object sender, System.Windows.RoutedEventArgs e)
		{
			var app = Application.Current as App;
			if (null != app)
			{
				var mapper = app.GetUriMapper();
				if( null != mapper )
				{
					var uri = mapper.MapUri(Source);
					ViewModel.SetUrl(uri);
				}
			}
		}

		private ViewModel ViewModel
		{
			get { return (ViewModel) DataContext; }
		}


		public static readonly DependencyProperty<View, Uri> SourceProperty =
			DependencyProperty<View, Uri>.Register(v => v.Source);
		public Uri Source
		{
			get { return SourceProperty.GetValue(this); }
			set { SourceProperty.SetValue(this, value); }
		}

	}
}
