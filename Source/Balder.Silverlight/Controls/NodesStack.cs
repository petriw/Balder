#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
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

using Balder.Core;
using Balder.Core.Helpers;
using Balder.Core.Math;

namespace Balder.Silverlight.Controls
{
	public class NodesStackModifier : INodeModifier
	{
		private readonly NodesStack _stack;

		public NodesStackModifier(NodesStack stack)
		{
			_stack = stack;
		}

		public void Apply(Node node, int nodeIndex, object dataContext)
		{
			Vector startPosition = _stack.StartPosition;
			Vector itemAdd = _stack.ItemAdd;

			var actualPosition = startPosition + (itemAdd*nodeIndex);
			node.Position = actualPosition;
		}
	}

	public class NodesStack : NodesControl
	{
		public NodesStack()
		{
			StartPosition = new Coordinate();
			ItemAdd = new Coordinate();
			var stackModifier = new NodesStackModifier(this);
			Modifier = stackModifier;
		}

		public static readonly DependencyProperty<NodesStack, Coordinate> StartPositionProperty =
			DependencyProperty<NodesStack, Coordinate>.Register(n => n.StartPosition);
		public Coordinate StartPosition
		{
			get { return StartPositionProperty.GetValue(this); }
			set
			{
				StartPositionProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static readonly DependencyProperty<NodesStack, Coordinate> ItemAddProperty =
			DependencyProperty<NodesStack, Coordinate>.Register(n => n.ItemAdd);
		public Coordinate ItemAdd
		{
			get { return ItemAddProperty.GetValue(this); }
			set
			{
				ItemAddProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}
	}
}
