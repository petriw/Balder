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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Balder.Silverlight.Notification
{
	public class NotifyingObjectWeaver
	{
		private const string DynamicAssemblyName = "Dynamic Assembly";
		private const string DynamicModuleName = "Dynamic Module";
		private const string PropertyChangedEventName = "PropertyChanged";
		private const string OnPropertyChangedMethodName = "OnPropertyChanged";

		private static readonly Type VoidType = typeof(void);
		private static readonly Type DelegateType = typeof(Delegate);

		private const MethodAttributes EventMethodAttributes =
			MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;

		private const MethodAttributes OnPropertyChangedMethodAttributes =
			MethodAttributes.Public | MethodAttributes.HideBySig;


		private static readonly AssemblyBuilder DynamicAssembly;
		private static readonly ModuleBuilder DynamicModule;

		private static readonly Dictionary<Type, Type> Proxies = new Dictionary<Type, Type>();

		static NotifyingObjectWeaver()
		{
			var dynamicAssemblyName = string.Format("{0}_{1}", DynamicAssemblyName, Guid.NewGuid());
			var dynamicModuleName = string.Format("{0}_{1}", DynamicModuleName, Guid.NewGuid());

			DynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(dynamicAssemblyName),
			                                                                AssemblyBuilderAccess.Run);

			DynamicModule = DynamicAssembly.DefineDynamicModule(dynamicModuleName);
		}

		public Type GetProxyType<T>()
		{
			var type = typeof(T);
			Type proxyType;
			if( Proxies.ContainsKey(type))
			{
				proxyType = Proxies[type];
			} else
			{
				proxyType = CreateProxyType(type);
				Proxies[type] = proxyType;
			}
			
			return proxyType;
		}

		private static Type CreateProxyType(Type type)
		{
			var typeBuilder = DefineType(type);
			var eventHandlerType = typeof(PropertyChangedEventHandler);

			DefineConstructorIfNoDefaultConstructorOnBaseType(type, typeBuilder);
			var propertyChangedFieldBuilder = typeBuilder.DefineField(PropertyChangedEventName, eventHandlerType, FieldAttributes.Private);
			DefineEvent(typeBuilder, eventHandlerType, propertyChangedFieldBuilder);
			var onPropertyChangedMethodBuilder = DefineOnPropertyChangedMethod(typeBuilder, eventHandlerType, propertyChangedFieldBuilder);
			DefineProperties(typeBuilder, type, onPropertyChangedMethodBuilder);

			var proxyType = typeBuilder.CreateType();
			return proxyType;
		}

		private static void DefineConstructorIfNoDefaultConstructorOnBaseType(Type type, TypeBuilder typeBuilder)
		{
			var constructors = type.GetConstructors();
			if( constructors.Length == 1 && constructors[0].GetParameters().Length == 0 )
			{
				return;
			}

			foreach( var constructor in constructors )
			{
				var parameters = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
				var constructorBuilder = typeBuilder.DefineConstructor(constructor.Attributes, constructor.CallingConvention, parameters);
				var constructorGenerator = constructorBuilder.GetILGenerator();
				constructorGenerator.Emit(OpCodes.Ldarg_0);

				for( var index=0; index<parameters.Length; index++ )
				{
					constructorGenerator.Emit(OpCodes.Ldarg,index+1);
				}
				constructorGenerator.Emit(OpCodes.Call, constructor);
				constructorGenerator.Emit(OpCodes.Nop);
				constructorGenerator.Emit(OpCodes.Nop);
				constructorGenerator.Emit(OpCodes.Nop);

				constructorGenerator.Emit(OpCodes.Ret);
			}
		}

		private static void DefineProperties(TypeBuilder typeBuilder, Type baseType, MethodBuilder onPropertyChangedMethodBuilder)
		{
			var properties = baseType.GetProperties();
			var query = from p in properties
			            where p.GetGetMethod().IsVirtual
			            select p;
			var virtualProperties = query.ToArray();
			foreach (var property in virtualProperties)
			{
				if( ShouldPropertyBeIgnored(property))
				{
					continue;
				}
				DefineGetMethodForProperty(property, typeBuilder);
				DefineSetMethodForProperty(property, typeBuilder, onPropertyChangedMethodBuilder);
			}
		}

		private static bool ShouldPropertyBeIgnored(PropertyInfo propertyInfo)
		{
			var attributes = propertyInfo.GetCustomAttributes(typeof (IgnoreChangesAttribute), true);
			return attributes.Length == 1;
		}

		private static void DefineSetMethodForProperty(PropertyInfo property, TypeBuilder typeBuilder, MethodBuilder onPropertyChangedMethodBuilder)
		{
			var setMethodToOverride = property.GetSetMethod();
			if( null == setMethodToOverride )
			{
				return;
			}
			var setMethodBuilder = typeBuilder.DefineMethod(setMethodToOverride.Name, setMethodToOverride.Attributes, VoidType, new[] { property.PropertyType });
			var setMethodGenerator = setMethodBuilder.GetILGenerator();
			var propertiesToNotifyFor = GetPropertiesToNotifyFor(property);


			setMethodGenerator.Emit(OpCodes.Nop);
			setMethodGenerator.Emit(OpCodes.Ldarg_0);
			setMethodGenerator.Emit(OpCodes.Ldarg_1);
			setMethodGenerator.Emit(OpCodes.Call, setMethodToOverride);
			
			foreach( var propertyName in propertiesToNotifyFor )
			{
				setMethodGenerator.Emit(OpCodes.Ldarg_0);
				setMethodGenerator.Emit(OpCodes.Ldstr, propertyName);
				setMethodGenerator.Emit(OpCodes.Call, onPropertyChangedMethodBuilder);
			}

			setMethodGenerator.Emit(OpCodes.Nop);
			setMethodGenerator.Emit(OpCodes.Ret);
			typeBuilder.DefineMethodOverride(setMethodBuilder, setMethodToOverride);
		}

		private static string[] GetPropertiesToNotifyFor(PropertyInfo property)
		{
			var properties = new List<string>();
			properties.Add(property.Name);

			var attributes = property.GetCustomAttributes(typeof (NotifyChangesForAttribute), true);
			foreach( NotifyChangesForAttribute attribute in attributes )
			{
				foreach( var propertyName in attribute.PropertyNames )
				{
					properties.Add(propertyName);
				}
			}
			return properties.ToArray();
		}

		private static void DefineGetMethodForProperty(PropertyInfo property, TypeBuilder typeBuilder)
		{
			var getMethodToOverride = property.GetGetMethod();
			var getMethodBuilder = typeBuilder.DefineMethod(getMethodToOverride.Name, getMethodToOverride.Attributes, property.PropertyType, new Type[0]);
			var getMethodGenerator = getMethodBuilder.GetILGenerator();
			var label = getMethodGenerator.DefineLabel();

			getMethodGenerator.DeclareLocal(property.PropertyType);
			getMethodGenerator.Emit(OpCodes.Nop);
			getMethodGenerator.Emit(OpCodes.Ldarg_0);
			getMethodGenerator.Emit(OpCodes.Call,getMethodToOverride);
			getMethodGenerator.Emit(OpCodes.Stloc_0);
			getMethodGenerator.Emit(OpCodes.Br_S, label);
			getMethodGenerator.MarkLabel(label);
			getMethodGenerator.Emit(OpCodes.Ldloc_0);
			getMethodGenerator.Emit(OpCodes.Ret);
			typeBuilder.DefineMethodOverride(getMethodBuilder, getMethodToOverride);
		}


		private static void DefineEvent(TypeBuilder typeBuilder, Type eventHandlerType, FieldBuilder fieldBuilder)
		{
			var eventBuilder = typeBuilder.DefineEvent("PropertyChanged", EventAttributes.None, eventHandlerType);
			DefineAddMethodForEvent(typeBuilder, eventHandlerType, fieldBuilder, eventBuilder);
			DefineRemoveMethodForEvent(typeBuilder, eventHandlerType, fieldBuilder, eventBuilder);
		}

		private static void DefineRemoveMethodForEvent(TypeBuilder typeBuilder, Type eventHandlerType, FieldBuilder fieldBuilder, EventBuilder eventBuilder)
		{
			var removeEventMethod = string.Format("remove_{0}", PropertyChangedEventName);
			var removeMethodInfo = DelegateType.GetMethod("Remove", BindingFlags.Public | BindingFlags.Static, null,
			                                              new[] { DelegateType, DelegateType }, null);
			var removeMethodBuilder = typeBuilder.DefineMethod(removeEventMethod, EventMethodAttributes, VoidType, new[] { eventHandlerType });
			var removeMethodGenerator = removeMethodBuilder.GetILGenerator();
			removeMethodGenerator.Emit(OpCodes.Ldarg_0);
			removeMethodGenerator.Emit(OpCodes.Ldarg_0);
			removeMethodGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
			removeMethodGenerator.Emit(OpCodes.Ldarg_1);
			removeMethodGenerator.EmitCall(OpCodes.Call, removeMethodInfo, null);
			removeMethodGenerator.Emit(OpCodes.Castclass, eventHandlerType);
			removeMethodGenerator.Emit(OpCodes.Stfld, fieldBuilder);
			removeMethodGenerator.Emit(OpCodes.Ret);
			eventBuilder.SetAddOnMethod(removeMethodBuilder);
		}

		private static void DefineAddMethodForEvent(TypeBuilder typeBuilder, Type eventHandlerType, FieldBuilder fieldBuilder, EventBuilder eventBuilder)
		{
			var combineMethodInfo = DelegateType.GetMethod("Combine", BindingFlags.Public | BindingFlags.Static, null,
			                                               new[] { DelegateType, DelegateType }, null);


			var addEventMethod = string.Format("add_{0}", PropertyChangedEventName);
			var addMethodBuilder = typeBuilder.DefineMethod(addEventMethod, EventMethodAttributes, VoidType, new[] { eventHandlerType });
			var addMethodGenerator = addMethodBuilder.GetILGenerator();
			addMethodGenerator.Emit(OpCodes.Ldarg_0);
			addMethodGenerator.Emit(OpCodes.Ldarg_0);
			addMethodGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
			addMethodGenerator.Emit(OpCodes.Ldarg_1);
			addMethodGenerator.EmitCall(OpCodes.Call, combineMethodInfo, null);
			addMethodGenerator.Emit(OpCodes.Castclass, eventHandlerType);
			addMethodGenerator.Emit(OpCodes.Stfld, fieldBuilder);
			addMethodGenerator.Emit(OpCodes.Ret);
			eventBuilder.SetAddOnMethod(addMethodBuilder);
		}

		private static MethodBuilder DefineOnPropertyChangedMethod(TypeBuilder typeBuilder, Type eventHandlerType, FieldBuilder fieldBuilder)
		{
			var propertyChangedEventArgsType = typeof(PropertyChangedEventArgs);

			var onPropertyChangedMethodBuilder = typeBuilder.DefineMethod(OnPropertyChangedMethodName, OnPropertyChangedMethodAttributes, VoidType,
			                                                              new[] { typeof(string) });
			var onPropertyChangedMethodGenerator = onPropertyChangedMethodBuilder.GetILGenerator();

			var label = onPropertyChangedMethodGenerator.DefineLabel();
			onPropertyChangedMethodGenerator.DeclareLocal(typeof (bool));
			onPropertyChangedMethodGenerator.Emit(OpCodes.Nop);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Ldnull);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Ldarg_0);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Ceq);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Stloc_0);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Ldloc_0);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Brtrue_S,label);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Nop);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Ldarg_0);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Ldarg_0);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Ldarg_1);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Newobj, propertyChangedEventArgsType.GetConstructor(new[] { typeof(string) }));
			var invokeMethod = eventHandlerType.GetMethod("Invoke");
			onPropertyChangedMethodGenerator.EmitCall(OpCodes.Callvirt, invokeMethod, null);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Nop);
			onPropertyChangedMethodGenerator.MarkLabel(label);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Nop);
			onPropertyChangedMethodGenerator.Emit(OpCodes.Ret);
			return onPropertyChangedMethodBuilder;
		}

		private static TypeBuilder DefineType(Type type)
		{
			var uid = Guid.NewGuid();
			var name = string.Format("{0}{1}", type.Name, uid);
			var typeBuilder = DynamicModule.DefineType(name);
			typeBuilder.SetParent(type);
			var interfaceType = typeof(INotifyPropertyChanged);
			typeBuilder.AddInterfaceImplementation(interfaceType);
			return typeBuilder;
		}
	}
}