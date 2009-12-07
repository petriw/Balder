using System.ComponentModel;
using System.Windows.Controls;
using Balder.Core.Helpers;
using Balder.Core.Math;
using Balder.Core.TypeConverters;

namespace Balder.Core
{
	[TypeConverter(typeof(VectorTypeConverter))]
	public partial class Node : Control
	{
		public static readonly DependencyProperty<Node, Vector> PositionProperty =
			DependencyProperty<Node, Vector>.Register(n => n.Position);
		public Vector Position
		{
			get { return PositionProperty.GetValue(this); }
			set { PositionProperty.SetValue(this,value); }
		}

		public Vector Scale { get; set; }
		public Matrix World { get; set; }
		
		public BoundingSphere BoundingSphere { get; set; }
		public bool IsVisible { get; set; }
		public Color Color { get; set; }
	}
}
