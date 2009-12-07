using Balder.Core.Math;

namespace Balder.Core
{
	interface INode
	{
		/// <summary>
		/// Get and set the position in space for the node
		/// </summary>
		Vector Position { get; set; }

		/// <summary>
		/// Get and set the scale of the node
		/// </summary>
		Vector Scale { get; set; }

		/// <summary>
		/// Get and set the matrix representing the node in the world
		/// </summary>
		Matrix World { get; set; }

		/// <summary>
		/// The bounding sphere surrounding the node
		/// </summary>
		BoundingSphere BoundingSphere { get; set; }

		/// <summary>
		/// Get and set wether or not the node is visible
		/// </summary>
		bool IsVisible { get; set; }

		/// <summary>
		/// Color of the node - this will be used if node supports it
		/// during lighting calculations. If Node has different ways of defining
		/// its color, for instance Materialing or similar - this color
		/// will most likely be overridden
		/// </summary>
		Color Color { get; set; }
	}
}
