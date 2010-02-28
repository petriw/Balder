using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Balder.Core.Execution;
using Balder.Core.Helpers;

namespace Balder.Core
{
	public class NodeCloneInfo
	{
		public Type Type { get; private set; }
		public PropertyInfo[] Properties { get; private set; }
		public DependencyProperty[] DependencyProperties { get; private set; }

		private FieldInfo[] _dependencyPropertyFields;

		public NodeCloneInfo(Type type)
		{
			Type = type;
			PrepareDependencyProperties();
			PrepareProperties();

		}

		private void PrepareProperties()
		{
			var properties = new List<PropertyInfo>();

			var propertiesInType = Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (var property in propertiesInType)
			{
				if (!ShouldPropertyBeIgnored(property))
				{
					properties.Add(property);
				}
			}

			Properties = properties.ToArray();
		}


		private bool ShouldPropertyBeIgnored(PropertyInfo property)
		{
			if (property.Name.Equals("Children") ||
				!property.CanWrite ||
				property.DeclaringType.FullName.StartsWith("System") 				
				)
			{
				return true;
			}
			return false;
		}

		private bool IsPropertyDependencyProperty(PropertyInfo property)
		{
			var propName = string.Format("{0}Prop", property.Name);
			var propertyName = string.Format("{0}Property", property.Name);
			var query = from f in _dependencyPropertyFields
						where f.Name.Equals(propName) || f.Name.Equals(propertyName)
						select f;
			var field = query.SingleOrDefault();
			return null != field;
		}

		public bool IsPropertyCloneable(PropertyInfo property)
		{
			if( !property.PropertyType.IsValueType )
			{
				var cloneableInterface = property.PropertyType.GetInterface(typeof (ICloneable).Name, false);
				return null != cloneableInterface;
			}
			return false;
		}

		private void PrepareDependencyProperties()
		{
			var dependencyPropertyFields = new List<FieldInfo>();
			var dependencyProperties = new List<DependencyProperty>();
			var fields = Type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
			foreach (var field in fields)
			{
				var value = field.GetValue(null);
				if (field.FieldType.IsGenericType)
				{
					var propertyType = typeof(Property<,>);
					var dependencyPropertyType = typeof(DependencyProperty<,>);
					if (field.FieldType.FullName.StartsWith(propertyType.FullName) ||
						field.FieldType.FullName.StartsWith(dependencyPropertyType.FullName))
					{
						var actualDependencyPropertyProperty = field.FieldType.GetProperty("ActualDependencyProperty");
						value = actualDependencyPropertyProperty.GetValue(value, null);
					}
				}
				if (value is DependencyProperty)
				{
					var dependencyProperty = value as DependencyProperty;
					dependencyProperties.Add(dependencyProperty);
					dependencyPropertyFields.Add(field);
				}
			}

			DependencyProperties = dependencyProperties.ToArray();
			_dependencyPropertyFields = dependencyPropertyFields.ToArray();
		}

	}
}