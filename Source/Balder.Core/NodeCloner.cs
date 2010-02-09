using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Balder.Core
{
	public class NodeCloner
	{
		public static readonly NodeCloner Instance = new NodeCloner();
		private readonly Dictionary<Type, NodeCloneInfo> _typeCloneInfo;

		private NodeCloner()
		{
			_typeCloneInfo = new Dictionary<Type, NodeCloneInfo>();
		}


		private NodeCloneInfo GetInfoForType(Type type)
		{
			NodeCloneInfo info;
			if (_typeCloneInfo.ContainsKey(type))
			{
				info = _typeCloneInfo[type];
			}
			else
			{
				info = new NodeCloneInfo(type);
				_typeCloneInfo[type] = info;
			}
			return info;
		}


		public Node Clone(Node source)
		{
			var type = source.GetType();
			var cloneInfo = GetInfoForType(type);

			var clone = Activator.CreateInstance(type) as Node;

			ClonePropertyValues(source, cloneInfo, clone);
			CloneBindingExpressions(source, cloneInfo, clone);

			foreach( var child in source.Children )
			{
				var clonedChild = child.Clone();
				clone.Children.Add(clonedChild);
			}

			return clone;

		}

		private static void ClonePropertyValues(Node source, NodeCloneInfo cloneInfo, Node clone)
		{
			foreach( var property in cloneInfo.Properties )
			{
				var value = property.GetValue(source, null);
				if( cloneInfo.IsPropertyCloneable(property))
				{
					value = ((Execution.ICloneable) value).Clone();
				}

				if (null != value)
				{
					property.SetValue(clone, value, null);
				}
			}
		}

		private static void CloneBindingExpressions(Node source, NodeCloneInfo cloneInfo, Node clone)
		{
			foreach( var dependencyProperty in cloneInfo.DependencyProperties )
			{
				var bindingExpression = source.GetBindingExpression(dependencyProperty);
				if( null != bindingExpression )
				{
					clone.SetBinding(dependencyProperty, bindingExpression.ParentBinding);
				}
			}
		}
	}
}