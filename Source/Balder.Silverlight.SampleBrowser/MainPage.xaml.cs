using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Balder.Core.Execution;
using Balder.Silverlight.Display;
using Balder.Silverlight.SoftwareRendering;

namespace Balder.Silverlight.SampleBrowser
{
	public partial class MainPage : UserControl
	{


		public MainPage()
		{
			InitializeComponent();

			BufferStatisticsGrid.DataContext = WriteableBitmapQueue.WriteableBitmapQueueStatistics.Instance;
			RenderStatisticsGrid.DataContext = RenderingStatistics.Instance;
		}

		private void HandleGameInVisualTree(UIElement element, bool reload)
		{
			if( null == element )
			{
				return;
			}
			if( element is ItemsControl )
			{
				foreach( var item in ((ItemsControl)element).Items )
				{
					if( item is Game )
					{
						HandleGame((Game)item, reload);
						break;
					} else if( item is UIElement)
					{
						HandleGameInVisualTree((UIElement)item, reload);
					}
				}
			} else if( element is Panel )
			{
				foreach( var child in ((Panel)element).Children )
				{
					if( child is Game )
					{
						HandleGame((Game)child, reload);
						break;
					} else
					{
						HandleGameInVisualTree(child, reload);
					}
				}
			} if( element is ContentControl )
			{
				var contentControl = element as ContentControl;
				if( contentControl.Content is Game )
				{
					HandleGame((Game)contentControl.Content, reload);
				} else if( contentControl.Content is UIElement )
				{
					HandleGameInVisualTree((UIElement)contentControl.Content, reload);
				}
			} else
			{
				try
				{
					var child = VisualTreeHelper.GetChild(element, 0);
					if (null != child && child is UIElement)
					{
						HandleGameInVisualTree(child as UIElement, reload);
					}
				} catch
				{
					// Todo: fix this in total.  Smells already 
				}
			}
		}


		private void HandleGame(Game game, bool reload)
		{
			if( reload )
			{
			} else
			{
				game.Unload();	
			}
		}



		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if( null == TabControl )
			{
				return;
			}
			if( TabControl.SelectedIndex != 0 )
			{
				HandleGameInVisualTree(SampleTabItem,false);
			} else
			{
				HandleGameInVisualTree(SampleTabItem, true);
			}
		}

		private void ContentFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			_resourceView.Source = ContentFrame.Source;
		}

		private void ContentFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
		{
			HandleGameInVisualTree(ContentFrame, false);
		}
	}
}
