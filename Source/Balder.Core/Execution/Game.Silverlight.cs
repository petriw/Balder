using System;
using System.Windows;

namespace Balder.Core.Execution
{
	public partial class Game
	{
		partial void Constructed()
		{
			Loaded += GameLoaded;
		}

		private void GameLoaded(object sender, RoutedEventArgs e)
		{
			Validate();
			RegisterGame();
			AddNodesToScene();
		}

		private void RegisterGame()
		{
			var display = Runtime.Instance.Platform.DisplayDevice.CreateDisplay();
			display.Initialize((int)Width,(int)Height);
			Runtime.Instance.RegisterGame(display,this);
			display.InitializeContainer(this);
		}

		private void Validate()
		{
			if (0 == Width || Width.Equals(double.NaN) ||
				0 == Height || Height.Equals(double.NaN))
			{
				throw new ArgumentException("You need to specify Width and Height");
			}
		}

		private void AddNodesToScene()
		{
			foreach (var element in Children)
			{
				if( element is Node )
				{
					Scene.AddNode(element as Node);	
				}
				
			}
		}
	}
}
