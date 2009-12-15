using System;
using System.Windows;
using System.Windows.Input;
using Balder.Core.Display;
using Balder.Core.View;

namespace Balder.Core.Execution
{
	public partial class Game
	{
		private Node _previousNode;
		private IDisplay _display;

		partial void Constructed()
		{
			Loaded += GameLoaded;
		}


		public void Unload()
		{
			Runtime.Instance.UnregisterGame(this);
		}

		public static readonly Property<Game, Camera> CameraProp = Property<Game, Camera>.Register(g => g.Camera);
		public Camera Camera
		{
			get { return CameraProp.GetValue(this); }
			set
			{
				var previousCamera = Camera;
				if( null != previousCamera )
				{
					if( Children.Contains(previousCamera) )
					{
						Children.Remove(previousCamera);
					}
				}
				CameraProp.SetValue(this, value);
				Viewport.View = value;

				Children.Add(value);
			}
		}


		private void GameLoaded(object sender, RoutedEventArgs e)
		{
			Validate();
			RegisterGame();
			AddNodesToScene();
			InitializeMouse();
		}

		private void RegisterGame()
		{
			_display = Runtime.Instance.Platform.DisplayDevice.CreateDisplay();
			_display.Initialize((int)Width,(int)Height);
			Runtime.Instance.RegisterGame(_display,this);
			_display.InitializeContainer(this);
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

		private void InitializeMouse()
		{
			MouseLeftButtonDown += MouseLeftButtonDownOccured;
			MouseLeftButtonUp += MouseLeftButtonUpOccured;
			MouseMove += MouseMoveOccured;
			MouseEnter += MouseEnterOccured;
			MouseLeave += MouseLeaveOccured;
		}

		private void MouseMoveOccured(object sender, MouseEventArgs e)
		{
			var position = e.GetPosition(this);
			var hitNode = Scene.GetNodeAtScreenCoordinate(Viewport, (int)position.X, (int)position.Y);
			if (null != hitNode)
			{
				if (null == _previousNode ||
					!hitNode.Equals(_previousNode))
				{
					CallActionOnSilverlightNode(hitNode, n => n.RaiseMouseEnter(e));
				}
			}
			else if (null != _previousNode)
			{
				CallActionOnSilverlightNode(_previousNode, n => n.RaiseMouseLeave(e));
			}

			_previousNode = hitNode;
		}

		private void MouseEnterOccured(object sender, MouseEventArgs e)
		{
			var position = e.GetPosition(this);
			var hitNode = Scene.GetNodeAtScreenCoordinate(Viewport, (int)position.X, (int)position.Y);
			if (null != hitNode)
			{
				if (null == _previousNode ||
					!hitNode.Equals(_previousNode))
				{
					CallActionOnSilverlightNode(hitNode, n => n.RaiseMouseEnter(e));
				}
			}
			_previousNode = hitNode;
		}

		private void MouseLeaveOccured(object sender, MouseEventArgs e)
		{
			if (null != _previousNode)
			{
				CallActionOnSilverlightNode(_previousNode, n => n.RaiseMouseLeave(e));
			}
		}

		private void MouseLeftButtonDownOccured(object sender, MouseButtonEventArgs e)
		{
			RaiseMouseEvent(e, n => n.RaiseMouseLeftButtonDown(e));
		}

		private void MouseLeftButtonUpOccured(object sender, MouseButtonEventArgs e)
		{
			RaiseMouseEvent(e, n => n.RaiseMouseLeftButtonUp(e));
		}

		private void RaiseMouseEvent(MouseEventArgs e, Action<Node> a)
		{
			var position = e.GetPosition(this);
			var hitNode = Scene.GetNodeAtScreenCoordinate(Viewport, (int)position.X, (int)position.Y);
			if (null != hitNode)
			{
				CallActionOnSilverlightNode(hitNode, a);
			}
		}

		private void CallActionOnSilverlightNode(Core.Node node, Action<Node> a)
		{
			foreach (var element in Children)
			{
				if (element is Node && !(element is IView))
				{
					a(node);
				}
			}

		}
	}
}
