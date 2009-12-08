using System.ComponentModel;
using System.Windows;
using Balder.Core.TypeConverters;

namespace Balder.Core.Math
{
	[TypeConverter(typeof(CoordinateTypeConverter))]
	public partial class Coordinate : DependencyObject
	{

	}
}
