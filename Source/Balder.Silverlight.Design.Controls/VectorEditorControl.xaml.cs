using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Balder.Silverlight.Design.Controls
{
	/// <summary>
	/// Interaction logic for VectorEditorControl.xaml
	/// </summary>
	public partial class VectorEditorControl : UserControl
	{
		public VectorEditorControl()
		{
			InitializeComponent();

			DataContextChanged += new DependencyPropertyChangedEventHandler(VectorEditorControl_DataContextChanged);
		}

		void VectorEditorControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			int i = 0;
			i++;
		}
	}
}
