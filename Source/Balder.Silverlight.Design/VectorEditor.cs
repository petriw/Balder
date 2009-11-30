using System.Windows;
using Balder.Silverlight.Design.Controls;
using Microsoft.Windows.Design.PropertyEditing;

namespace Balder.Silverlight.Design
{
	
	public class VectorEditor : PropertyValueEditor
	{
		public VectorEditor(DataTemplate dataTemplate)
			: base(dataTemplate)
		{
			
		}

		public VectorEditor()
		{
			var vectorEditControl = new FrameworkElementFactory(typeof (VectorEditorControl));
			var dataTemplate = new DataTemplate {VisualTree = vectorEditControl};
			InlineEditorTemplate = dataTemplate;
		}
	}
}
