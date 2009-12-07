using System.ComponentModel;
using System.Windows.Controls;
using Balder.Core.Execution;
using Balder.Core.Math;
using Balder.Core.TypeConverters;

namespace Balder.Core
{
	[TypeConverter(typeof(VectorTypeConverter))]
	public partial class Node : Control, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
		public static readonly Property<Node, Vector> PositionProp = Property<Node, Vector>.Register(n => n.Position);
		public Vector Position
		{
			get { return PositionProp.GetValue(this); }
			set { PositionProp.SetValue(this, value); }
		}

		public Vector Scale { get; set; }
		public Matrix World { get; set; }

		public BoundingSphere BoundingSphere { get; set; }
		public bool IsVisible { get; set; }
		public Color Color { get; set; }

	}
}
