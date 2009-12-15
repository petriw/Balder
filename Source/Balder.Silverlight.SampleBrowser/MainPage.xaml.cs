using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Balder.Core;
using Balder.Core.Execution;

namespace Balder.Silverlight.SampleBrowser
{
	public partial class MainPage : UserControl
	{
		public MainPage()
		{
			InitializeComponent();

			


		}

		private void RemoveGameInVisualTree(UIElement element)
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
						RemoveGame((Game)item);
						break;
					} else if( item is UIElement)
					{
						RemoveGameInVisualTree((UIElement)item);
					}
				}
			} else if( element is Panel )
			{
				foreach( var child in ((Panel)element).Children )
				{
					if( child is Game )
					{
						RemoveGame((Game)child);
						break;
					} else
					{
						RemoveGameInVisualTree(child);
					}
				}
			} if( element is ContentControl )
			{
				var contentControl = element as ContentControl;
				if( contentControl.Content is Game )
				{
					RemoveGame((Game) contentControl.Content);
				} else if( contentControl.Content is UIElement )
				{
					RemoveGameInVisualTree((UIElement)contentControl.Content);
				}
			} else
			{
				try
				{
					var child = VisualTreeHelper.GetChild(element, 0);
					if (null != child && child is UIElement)
					{
						RemoveGameInVisualTree(child as UIElement);
					}
				} catch
				{
					// Todo: fix this in total.  Smells already 
				}
			}
		}

		private void RemoveGame(Game game)
		{
			game.Unload();
			/*
			var parent = VisualTreeHelper.GetParent(game);
			if( null != parent )
			{
				if( parent is ItemsControl )
				{
					((ItemsControl) parent).Items.Remove(game);
				} else if( parent is Panel )
				{
					((Panel) parent).Children.Remove(game);
				} else if( parent is ContentControl )
				{
					((ContentControl) parent).Content = null;
				}
			}
			 */
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			foreach( TabItem tabItem in e.RemovedItems )
			{
				RemoveGameInVisualTree(tabItem);
			}
		}

		private void ContentFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			

		}

		private void ContentFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
		{
			RemoveGameInVisualTree(ContentFrame);
		}
	}
}
