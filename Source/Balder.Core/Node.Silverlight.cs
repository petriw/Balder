using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Balder.Core.Execution;
using Balder.Core.TypeConverters;
using Balder.Core.Helpers;

namespace Balder.Core
{
	public partial class Node : ItemsControl
	{
		public new event MouseEventHandler MouseMove = (s, e) => { };
		public new event MouseEventHandler MouseEnter = (s, e) => { };
		public new event MouseEventHandler MouseLeave = (s, e) => { };
		public new event MouseButtonEventHandler MouseLeftButtonDown = (s, e) => { };
		public new event MouseButtonEventHandler MouseLeftButtonUp = (s, e) => { };


		partial void Initialize()
		{
			Loaded += NodeLoaded;
			Children.CollectionChanged += ChildrenChanged;
		}

		private void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch( e.Action )
			{
				case NotifyCollectionChangedAction.Add:
					{
						foreach( var item in e.NewItems )
						{
							Items.Add(item);
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					{
						foreach( var item in e.OldItems )
						{
							Items.Remove(item);
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					{
						Items.Clear();
					}
					break;

			}
		}

		private void NodeLoaded(object sender, RoutedEventArgs e)
		{
			Runtime.Instance.WireUpDependencies(this);
			OnLoaded();
		}


		public static readonly Property<Node, Color> ColorProp = Property<Node, Color>.Register(n => n.Color);
		[TypeConverter(typeof(ColorConverter))]
		public Color Color
		{
			get { return ColorProp.GetValue(this); }
			set
			{
				ColorProp.SetValue(this, value);
				SetColorForChildren();
			}
		}

		public static readonly DependencyProperty<Node, ICommand> CommandProperty =
			DependencyProperty<Node, ICommand>.Register(o => o.Command);
		public ICommand Command
		{
			get { return CommandProperty.GetValue(this); }
			set { CommandProperty.SetValue(this, value); }
		}

		public static readonly DependencyProperty<Node, object> CommandParameterProperty =
			DependencyProperty<Node, object>.Register(o => o.CommandParameter);
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

		internal virtual void RaiseMouseMove(MouseEventArgs e)
		{
			MouseMove(this, e);
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
			MouseLeave(this, e);
		}


		protected virtual void OnLoaded() { }
	}
}
