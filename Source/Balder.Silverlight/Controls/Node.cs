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
using System.Windows.Input;
using Balder.Silverlight.Controls.Math;
using Balder.Silverlight.Helpers;
using Balder.Silverlight.Interaction;

namespace Balder.Silverlight.Controls
{
	public class Node : BalderControl, ICommandSource
	{
		public new event MouseEventHandler MouseEnter = (s, e) => { };
		public new event MouseEventHandler MouseLeave = (s, e) => { };
		public new event MouseButtonEventHandler MouseLeftButtonDown = (s, e) => { };
		public new event MouseButtonEventHandler MouseLeftButtonUp = (s, e) => { };


		private Core.Node _actualNode;
		public Core.Node ActualNode
		{
			get { return _actualNode; }
			protected set
			{
				_actualNode = value;
				if( null == Position )
				{
					Position = new Vector();	
				}
				InitializePosition();
			}
		}

		private void InitializePosition()
		{
			Position.SetNativeAction((x, y, z) =>
			{
				if (null != ActualNode)
				{
					ActualNode.Position.X = x;
					ActualNode.Position.Y = y;
					ActualNode.Position.Z = z;
				}
			});
			
		}

		public static readonly DependencyProperty<Node, Vector> PositionProperty =
			DependencyProperty<Node, Vector>.Register(o => o.Position);
		public Vector Position
		{
			get { return PositionProperty.GetValue(this); }
			set
			{
				PositionProperty.SetValue(this, value);
				if( null != ActualNode )
				{
					InitializePosition();
				}
			}
		}


		public static readonly DependencyProperty<Geometry, ICommand> CommandProperty =
			DependencyProperty<Geometry, ICommand>.Register(o => o.Command);
		public ICommand Command
		{
			get { return CommandProperty.GetValue(this); }
			set { CommandProperty.SetValue(this, value); }
		}

		public static readonly DependencyProperty<Geometry, object> CommandParameterProperty =
			DependencyProperty<Geometry, object>.Register(o => o.CommandParameter);
		public object CommandParameter
		{
			get { return CommandParameterProperty.GetValue(this); }
			set { CommandParameterProperty.SetValue(this, value); }
		}

		protected void OnCommand()
		{
			if (null != Command)
			{
				if (Command.CanExecute(CommandParameter))
				{
					Command.Execute(CommandParameter);
				}
			}
		}

		internal virtual void RaiseMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			OnCommand();
			MouseLeftButtonUp(this, e);
		}

		internal virtual void RaiseMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			MouseLeftButtonDown(this, e);
		}

		internal virtual void RaiseMouseEnter(MouseEventArgs e)
		{
			MouseEnter(this, e);
		}

		internal virtual void RaiseMouseLeave(MouseEventArgs e)
		{
			MouseEnter(this, e);
		}
	}
}
