using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
#if(!SILVERLIGHT)
using System.Runtime.Serialization;
#endif

namespace Balder.Core.Utils
{
	public class Cloner
	{
		public static T DeepClone<T>(T o)
		{
			var type = typeof (T);
			var obj = (T)DeepClone(type, o);
			return obj;
		}

		public static void DeepClone<T>(T destination, T source)
		{
			var type = typeof (T);
			DeepClone(type, destination, source);
		}

		public static object DeepClone(Type type, object source)
		{
			var objectTracker = new Dictionary<int, object>();
			var destination = DeepClone(type, source, objectTracker);
			return destination;
		}

		public static void DeepClone(Type type, object destination, object source)
		{
			var objectTracker = new Dictionary<int, object>();
			DeepClone(type, destination, source, objectTracker);
		}

		private static object DeepClone(Type type, object source, IDictionary<int, object> objectTracker)
		{
			var destination = CreateInstance(type);
			DeepClone(type, destination, source, objectTracker);
			return destination;
		}

		private static void DeepClone(Type type, object destination, object source, IDictionary<int, object> objectTracker)
		{
			var hashCode = source.GetHashCode();
			objectTracker[hashCode] = destination;
			CloneFields(type, destination, source, objectTracker);
			CloneProperties(type, destination, source, objectTracker);
		}

		private static object CreateInstance(Type type)
		{
			var instance = Activator.CreateInstance(type);
			return instance;
		}

		private static void CloneFields(Type type, object target, object source, IDictionary<int, object> objectTracker)
		{
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach( var field in fields )
			{
				if( ShouldIgnore(field))
				{
					continue;
				}
				var value = field.GetValue(source);
				value = HandleValue(field.FieldType, value, objectTracker);
				field.SetValue(target,value);
			}
		}

		private static void CloneProperties(Type type, object target, object source, IDictionary<int, object> objectTracker)
		{
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach( var property in properties )
			{
				if( source is IEnumerable ||
				    !property.CanWrite ||
				    ShouldIgnore(property))
				{
					continue;
				}

				var indexParameters = property.GetIndexParameters();
				if( indexParameters.Length > 0 && !property.PropertyType.IsArray )
				{
					continue;
				}


				var value = property.GetValue(source, null);
				value = HandleValue(property.PropertyType, value, objectTracker);
				property.SetValue(target, value, null);
			}
		}


		private static object HandleValue(Type valueType, object value, IDictionary<int, object> objectTracker)
		{
			if( null == value )
			{
				return null;
			}
			if( IsString(valueType))
			{
				return value;
			}
			var hashCode = value.GetHashCode();
			if (objectTracker.ContainsKey(hashCode))
			{
				value = objectTracker[hashCode];
			} else if( IsEnumerable(valueType) )
			{
				value = HandleEnumerable(valueType, value as IEnumerable, objectTracker);
			} else if (!valueType.IsValueType)
			{
				value = DeepClone(valueType, value, objectTracker);
			}
			return value;
		}

		private static bool ShouldIgnore(ICustomAttributeProvider memberInfo)
		{
			var xmlIgnoreAttributes = memberInfo.GetCustomAttributes(typeof (XmlIgnoreAttribute), true);
			if( xmlIgnoreAttributes.Length == 1 )
			{
				return true;
			}
#if(!SILVERLIGHT)
			var ignoreDataMemberAttributes = memberInfo.GetCustomAttributes(typeof (IgnoreDataMemberAttribute), true);
			if( ignoreDataMemberAttributes.Length == 1 )
			{
				return true;
			}
#endif
			return false;
		}

		private static bool IsEnumerable(Type valueType)
		{
			var enumerableInterface = valueType.GetInterface(typeof (IEnumerable).Name, false);
			return enumerableInterface != null;
		}

		private static bool IsList(Type valueType)
		{
			var listInterface = valueType.GetInterface(typeof (IList).Name, false);
			return listInterface != null;
		}

		private static bool IsArray(Type valueType)
		{
			return valueType.IsArray;
		}

		private static bool IsString(Type valueType)
		{
			return valueType.Equals(typeof (string));
		}

		private static IEnumerable HandleEnumerable(Type originalType, IEnumerable enumerable, IDictionary<int, object> objectTracker)
		{
			IEnumerable target = null;
			var type = enumerable.GetType();
			if( IsArray(type))
			{
				var elementType = type.GetElementType();
				var sourceArray = (Array) enumerable;
				var targetArray = Array.CreateInstance(elementType, sourceArray.Length);
				Array.Copy(sourceArray, targetArray,sourceArray.Length);
				target = targetArray;
				
			} else if( IsList(type))
			{
				target = CloneList(enumerable, originalType, objectTracker);
			}
			return target;
		}

		private static IEnumerable CloneList(IEnumerable enumerable, Type originalType, IDictionary<int, object> objectTracker)
		{
			IEnumerable target;
			var listType = typeof (List<>);
			var elementType = typeof (object);

			if( originalType.IsGenericType) 
			{
				var genericArguments = originalType.GetGenericArguments();
				elementType = genericArguments[0];
			} else
			{
				var enumerator = enumerable.GetEnumerator();
				if( enumerator.MoveNext() )
				{
					var current = enumerator.Current;
					elementType = current.GetType();

					enumerator.Reset();
				}
			}

			var genericListType = listType.MakeGenericType(elementType);
			target = Activator.CreateInstance(genericListType) as IList;
				
			CloneContentOfEnumerable(target as IList, enumerable, objectTracker);
			return target;
		}

		private static void CloneContentOfEnumerable(IList target, IEnumerable source, IDictionary<int, object> objectTracker)
		{
			foreach (var element in source)
			{
				var elementType = element.GetType();
				var clone = HandleValue(elementType, element, objectTracker);
				target.Add(clone);
			}
		}
	}
}