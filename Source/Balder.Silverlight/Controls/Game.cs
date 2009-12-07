#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Balder.Core;
using Balder.Core.Debug;
using Balder.Core.Display;
using Balder.Core.Helpers;
using Balder.Silverlight.Converters;
using Balder.Silverlight.Input;
using Color=System.Windows.Media.Color;

namespace Balder.Silverlight.Controls
{

	public class Game : BalderControl
	{
		private static readonly DebugLevelConverter _debugLevelConverter = new DebugLevelConverter();
		private Image _image;
		private Color _previousBackgroundColor;
		private Core.Node _previousNode;

		protected override void OnLoaded()
		{
			Validate();

			Platform.DisplayDevice.Update += DisplayDevice_Update;
			InitializeFromPlatform();
			InitializeGame();
			InitializeContent();
			InitializeMouse();
			InitializeDebugLevel();

			if( Platform.IsInDesignMode )
			{
				Render();
			}

			base.OnLoaded();
		}

		public void Render()
		{
			if (Platform.DisplayDevice is Display.DisplayDevice)
			{
				var displayDevice = Platform.DisplayDevice as Display.DisplayDevice;
				var display = Display as Display.Display;
				displayDevice.RenderAndShow(display);
			}
		}


		void DisplayDevice_Update(IDisplay display)
		{
			if (_previousBackgroundColor.Equals(display.BackgroundColor))
			{
				SetBackgroundColor();
			}
		}

		private void SetBackgroundColor()
		{
			var color = Display.BackgroundColor.ToSystemColor();
			Background = new SolidColorBrush(color);
			_previousBackgroundColor = color;
		}


		private void Validate()
		{
			if (0 == Width || Width.Equals(double.NaN) ||
				0 == Height || Height.Equals(double.NaN))
			{
				throw new ArgumentException("You need to specify Width and Height");
			}
		}

		private void InitializeGame()
		{
			if( null == GameClass )
			{
				GameClass = new InternalGame();
			}
			Runtime.RegisterGame(Display, GameClass);

			Viewport = GameClass.Viewport;
			Scene = GameClass.Scene;
			if( null == Camera )
			{
				Camera = new Camera(GameClass.Camera);	
			} else
			{
				Camera.ActualCamera = GameClass.Camera;
			}
			
		}

		private void InitializeFromPlatform()
		{
			Display = Platform.DisplayDevice.CreateDisplay();
			Display.Initialize((int)Width, (int)Height);
			if (Platform.MouseDevice is MouseDevice)
			{
				((MouseDevice)Platform.MouseDevice).Initialize(this);
			}
		}

		private void InitializeContent()
		{
			if (Display is Display.Display)
			{
				_image = new Image
							{
								Source = ((Display.Display)Display).FramebufferBitmap,
								Stretch = Stretch.None
							};
				Children.Add(_image);

				SetBackgroundColor();
			}

			AddNodesToScene();
		}

		private void InitializeMouse()
		{
			MouseLeftButtonDown += MouseLeftButtonDownOccured;
			MouseLeftButtonUp += MouseLeftButtonUpOccured;
			MouseMove += MouseMoveOccured;
			MouseEnter += MouseEnterOccured;
			MouseLeave += MouseLeaveOccured;
		}

		private void InitializeDebugLevel()
		{
			var debugLevel = (DebugLevel)_debugLevelConverter.ConvertFromString(DebugLevel);
			Core.Runtime.Instance.DebugLevel = debugLevel;
		}


		private void MouseMoveOccured(object sender, MouseEventArgs e)
		{
			var position = e.GetPosition(this);
			var hitNode = Scene.GetNodeAtScreenCoordinate(Viewport, (int)position.X, (int)position.Y);
			if( null != hitNode )
			{
				if( null == _previousNode ||
					!hitNode.Equals(_previousNode))
				{
					CallActionOnSilverlightNode(hitNode,n=>n.RaiseMouseEnter(e));
				} 
			} else if( null != _previousNode )
			{
				CallActionOnSilverlightNode(_previousNode,n=>n.RaiseMouseLeave(e));
			}

			_previousNode= hitNode;
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
			RaiseMouseEvent(e,n=>n.RaiseMouseLeftButtonUp(e));
		}

		private void RaiseMouseEvent(MouseEventArgs e, Action<Node> a)
		{
			var position = e.GetPosition(this);
			var hitNode = Scene.GetNodeAtScreenCoordinate(Viewport, (int)position.X, (int)position.Y);
			if (null != hitNode)
			{
				CallActionOnSilverlightNode(hitNode,a);
			}
		}

		private void CallActionOnSilverlightNode(Core.Node node, Action<Node> a)
		{
			foreach (var element in Children)
			{
				if (element is Node && !(element is Camera))
				{
					var silverlightNode = element as Node;
					if (null != silverlightNode.ActualNode && silverlightNode.ActualNode == node)
					{
						a(silverlightNode);
					}
				}
			}
			
		}

		private void AddNodesToScene()
		{
			foreach (var element in Children)
			{
				if (element is Node && !(element is Camera))
				{
					var node = element as Node;
					if (null != node.ActualNode)
					{
						Scene.AddNode(node.ActualNode);
					}
				}
			}

		}

		public IDisplay Display { get; private set; }
		public Scene Scene { get; private set; }
		public Viewport Viewport { get; private set; }

		
		public DependencyProperty<Game, string> DebugLevelProperty =
			DependencyProperty<Game, string>.Register(g => g.DebugLevel);
		//[TypeConverter(typeof(DebugLevelConverter))]

		

		public string DebugLevel
		{
			get { return DebugLevelProperty.GetValue(this); }
			set
			{
				DebugLevelProperty.SetValue(this,value);
				
			}
		}


		public DependencyProperty<Game, Core.Execution.Game> GameClassProperty =
			DependencyProperty<Game, Core.Execution.Game>.Register(g => g.GameClass);
		public Core.Execution.Game GameClass
		{
			get { return GameClassProperty.GetValue(this); }
			set { GameClassProperty.SetValue(this, value); }
		}


		public DependencyProperty<Game, Camera> CameraProperty =
			DependencyProperty<Game, Camera>.Register(g => g.Camera);
		public Camera Camera
		{
			get { return CameraProperty.GetValue(this); }
			set { CameraProperty.SetValue(this, value); }
		}
	}
}
