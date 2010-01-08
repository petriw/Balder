#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
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
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using Balder.Core;
using Balder.Core.Helpers;

namespace Balder.Silverlight.Controls
{
	public class NodesControl : RenderableNode
	{
		public NodesControl()
		{
			Loaded += NodesControlLoaded;
		}

		
		private void NodesControlLoaded(object sender, RoutedEventArgs e)
		{
			PopulateFromItemsSource();
		}

		public static readonly DependencyProperty<NodesControl, DataTemplate> NodeTemplateProperty =
			DependencyProperty<NodesControl, DataTemplate>.Register(n => n.NodeTemplate);
		public DataTemplate NodeTemplate
		{
			get { return NodeTemplateProperty.GetValue(this); }
			set { NodeTemplateProperty.SetValue(this, value); }
		}


		public static readonly DependencyProperty<NodesControl, IEnumerable> ItemsSourceProperty =
			DependencyProperty<NodesControl, IEnumerable>.Register(n => n.ItemsSource);

		private IEnumerable _itemsSource;
		public IEnumerable ItemsSource
		{
			get { return ItemsSourceProperty.GetValue(this); }
			set
			{
				HandlePreviousItemsSource();
				_itemsSource = value;
				ItemsSourceProperty.SetValue(this, value);
				HandleNewItemsSource();

			}
		}

		private void HandleNewItemsSource()
		{

			
			
		}

		private void HandlePreviousItemsSource()
		{
			if (null != _itemsSource && _itemsSource is INotifyCollectionChanged )
			{
				var notifyCollectionChanged = _itemsSource as INotifyCollectionChanged;

			}
		}



		private void PopulateFromItemsSource()
		{
			
			if( null != NodeTemplate )
			{
				
				foreach (var item in _itemsSource)
				{
					var content = NodeTemplate.LoadContent() as Node;
					if (null == content)
					{
						throw new ArgumentException("Content of the template for NodeTemplate must be a derivative of Node");
					}

					content.DataContext = item;
					Children.Add(content);
				}
			}
		}
	}
}
