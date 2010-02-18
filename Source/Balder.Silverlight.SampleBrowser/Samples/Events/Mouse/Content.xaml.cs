using Balder.Core;

namespace Balder.Silverlight.SampleBrowser.Samples.Events.Mouse
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();
		}

		private void Mesh_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_mouseEnter.Text = "true";
			var node = sender as Node;
			if( null != node )
			{
				if( !string.IsNullOrEmpty(node.Name) )
				{
					_object.Text = node.Name;
				}
			}
		}

		private void Mesh_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_mouseEnter.Text = "false";
			_object.Text = "None";
		}

		private void Mesh_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_mouseLButtonUp.Text = "true";
			_mouseLButtonDown.Text = "false";
		}

		private void Mesh_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_mouseLButtonUp.Text = "false";
			_mouseLButtonDown.Text = "true";
		}

		private void LayoutRoot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			var translation = _infoBoxTranslation;
			var position = e.GetPosition(LayoutRoot);
			translation.X = position.X;
			translation.Y = position.Y;
			_xpos.Text = translation.X.ToString();
			_ypos.Text = translation.Y.ToString();
		}

		private void Mesh_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_mouseEnter.Text = "true";
			var node = sender as Node;
			if (null != node)
			{
				if (!string.IsNullOrEmpty(node.Name))
				{
					_object.Text = node.Name;
				}
			}
		}
	}
}
