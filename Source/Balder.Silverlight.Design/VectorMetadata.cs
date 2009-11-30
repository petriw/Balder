using Balder.Silverlight.Controls;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;

[assembly: ProvideMetadata(typeof(Balder.Silverlight.Design.VectorMetadata))]

namespace Balder.Silverlight.Design
{
	public class VectorMetadata : IProvideAttributeTable
	{
		public VectorMetadata()
		{
			var builder = new AttributeTableBuilder();

			
			builder.AddCustomAttributes(typeof (Node), 
										"Position",
			                            PropertyValueEditor.CreateEditorAttribute(typeof (VectorEditor)),
										new AlternateContentPropertyAttribute()
										);


			AttributeTable = builder.CreateTable();
		}

		public AttributeTable AttributeTable { get; private set; }
	}
}
