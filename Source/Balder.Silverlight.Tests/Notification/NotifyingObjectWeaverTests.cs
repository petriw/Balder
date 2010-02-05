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

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using Balder.Core;
using Balder.Core.Display;
using Balder.Silverlight.Notification;
using CThru.Silverlight;
using Moq;
using NUnit.Framework;

namespace Balder.Silverlight.Tests.Notification
{
	[TestFixture]
	public class NotifyingObjectWeaverTests
	{
		#region Stubs
		public class SimpleObject
		{

		}

		public class SimpleObjectOverridingToString
		{
			public const string ToStringReturn = "Something something something - Dark Side";

			public override string ToString()
			{
				return ToStringReturn;
			}
		}

		public class MyViewModel
		{
			public virtual int SomeInt { get; set; }
			public virtual string SomeString { get; set; }

			public bool SomeStringWithImplementationGetCalled;
			public bool SomeStringWithImplementationSetCalled;
			private string _someStringWithImplementation;
			public virtual string SomeStringWithImplementation
			{
				get
				{
					SomeStringWithImplementationGetCalled = true;
					return _someStringWithImplementation;

				}
				set
				{
					SomeStringWithImplementationSetCalled = true;
					_someStringWithImplementation = value;
				}
			}

			public virtual string PropertyWithPrivateSetter { get; private set; }
			public void SetPropertyWithPrivateSetter(string value)
			{
				PropertyWithPrivateSetter = value;
			}

			[IgnoreChanges]
			public virtual string IgnoredProperty { get; set; }

			[NotifyChangesFor("SomeString")]
			public virtual string SomeOtherString { get; set; }
		}

		public class MyViewModelWithConstructorDependencies
		{
			public MyViewModel ConstructorArgument1;
			public MyViewModel ConstructorArgument2;

			public MyViewModelWithConstructorDependencies(MyViewModel argument1, MyViewModel argument2)
			{
				ConstructorArgument1 = argument1;
				ConstructorArgument2 = argument2;
			}
		}

		public class Dispatcher : IDispatcher
		{
			public bool CheckAccessReturnValue = true;
			public bool CheckAccess()
			{
				return CheckAccessReturnValue;
			}

			public bool BeginInvokeDelegateCalled = false;
			public void BeginInvoke(Delegate del, params object[] arguments)
			{

				del.DynamicInvoke(arguments);
				BeginInvokeDelegateCalled = true;

			}

			public bool BeginInvokeActionCalled = false;
			public void BeginInvoke(Action a)
			{
				a();
				BeginInvokeActionCalled = true;
			}
		}

		public interface ISomeInterface
		{
			void SomeMethod();
		}

		public class TypeImplementingOtherInterface : ISomeInterface
		{
			public void SomeMethod()
			{

			}
		}

		public class TypeImplementingInterfaceInOtherAssembly : IDisplay
		{
			public Color BackgroundColor { get; set; }
			public virtual Color VirtualBackgroundColor { get; set; }

			public void Initialize(int width, int height)
			{
			}

			public void InitializeContainer(object container)
			{
			}
		}

		#endregion


		[SetUp]
		public void BeforeTest()
		{
			NotifyingObjectWeaver.ClearTypeCache();
		}

		[Test]
		public void CreatedTypeShouldInheritFromTypeGiven()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			Assert.That(type.BaseType, Is.EqualTo(typeof(MyViewModel)));
		}

		[Test]
		public void CreatedTypeShouldImplementINotifyPropertyChanged()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var interfaceType = type.GetInterface(typeof(INotifyPropertyChanged).Name, true);
			Assert.That(interfaceType, Is.Not.Null);
		}

		[Test, SilverlightUnitTest]
		public void CreatingInstanceOfTypeShouldNotCauseException()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type);
		}

		[Test]
		public void CallingGeneratedOnPropertyChangedMethodShouldFirePropertyChangedEvent()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var fired = false;
			instance.PropertyChanged += (s, e) => fired = true;
			var method = type.GetMethod("OnPropertyChanged");
			method.Invoke(instance, new[] { "Something" });
			Assert.That(fired, Is.True);
		}

		[Test]
		public void SettingIntegerPropertyShouldReturnSameValueWhenGetting()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var myViewModel = instance as MyViewModel;
			var expected = 5;
			myViewModel.SomeInt = expected;
			var actual = myViewModel.SomeInt;
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void SettingIntegerPropertyShouldFirePropertyChangedEvent()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var myViewModel = instance as MyViewModel;
			var fired = false;
			instance.PropertyChanged += (s, e) =>
											{
												if (e.PropertyName.Equals("SomeInt"))
												{
													fired = true;
												}
											};

			myViewModel.SomeInt = 5;
			Assert.That(fired, Is.True);
		}

		[Test]
		public void SettingStringPropertyShouldReturnSameValueWhenGetting()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var myViewModel = instance as MyViewModel;
			var expected = "Hello world";
			myViewModel.SomeString = expected;
			var actual = myViewModel.SomeString;
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void SettingStringPropertyShouldFirePropertyChangedEvent()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var myViewModel = instance as MyViewModel;
			var fired = false;
			instance.PropertyChanged += (s, e) =>
											{
												if (e.PropertyName.Equals("SomeString"))
												{
													fired = true;
												}
											};

			myViewModel.SomeString = "Hello world";
			Assert.That(fired, Is.True);
		}

		[Test]
		public void SettingPropertyShouldCallBaseClassSetter()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var myViewModel = instance as MyViewModel;
			myViewModel.SomeStringWithImplementation = "Something";
			Assert.That(myViewModel.SomeStringWithImplementationSetCalled, Is.True);
		}

		[Test]
		public void GettingPropertyShouldCallBaseClassGetter()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var myViewModel = instance as MyViewModel;
			myViewModel.SomeStringWithImplementation = "Something";
			var value = myViewModel.SomeStringWithImplementation;
			Assert.That(myViewModel.SomeStringWithImplementationGetCalled, Is.True);
		}

		[Test]
		public void PropertyDecoratedWithIgnoreChangesShouldNotFirePropertyChangedEvent()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var myViewModel = instance as MyViewModel;
			var fired = false;
			instance.PropertyChanged += (s, e) =>
											{
												if (e.PropertyName.Equals("IgnoredProperty"))
												{
													fired = true;
												}
											};
			myViewModel.IgnoredProperty = "Something";
			Assert.That(fired, Is.False);
		}

		[Test]
		public void PropertyDecoratedWithNotificationForMorePropertiesShouldFirePropertyChangedEventForAllProperties()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var myViewModel = instance as MyViewModel;
			var someStringFired = false;
			var someOtherStringFired = false;
			instance.PropertyChanged += (s, e) =>
											{
												if (e.PropertyName.Equals("SomeOtherString"))
												{
													someOtherStringFired = true;
												}
												if (e.PropertyName.Equals("SomeString"))
												{
													someStringFired = true;
												}
											};
			myViewModel.SomeOtherString = "Hello";
			Assert.That(someOtherStringFired, Is.True);
			Assert.That(someStringFired, Is.True);
		}

		[Test]
		public void PropertyWithPrivateSetterShouldBeIgnored()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var myViewModel = instance as MyViewModel;
			var fired = false;
			instance.PropertyChanged += (s, e) =>
											{
												if (e.PropertyName.Equals("PropertyWithPrivateSetter"))
												{
													fired = true;
												}
											};
			myViewModel.SetPropertyWithPrivateSetter("Something");
			Assert.That(fired, Is.False);
		}

		[Test]
		public void GettingProxyForSameTypeTwiceShouldYieldSameProxyType()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var firstType = weaver.GetProxyType<MyViewModel>();
			var secondType = weaver.GetProxyType<MyViewModel>();
			Assert.That(firstType, Is.EqualTo(secondType));
		}

		[Test]
		public void ObjectWithNonDefaultConstructorShouldHaveItsArgumentsForwardedFromProxy()
		{
			DispatcherManager.Current = new Dispatcher();
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModelWithConstructorDependencies>();

			var argument1 = new MyViewModel();
			var argument2 = new MyViewModel();

			var instance = Activator.CreateInstance(type, argument1, argument2) as MyViewModelWithConstructorDependencies;

			Assert.That(instance.ConstructorArgument1, Is.Not.Null);
			Assert.That(instance.ConstructorArgument1, Is.EqualTo(argument1));
			Assert.That(instance.ConstructorArgument2, Is.Not.Null);
			Assert.That(instance.ConstructorArgument2, Is.EqualTo(argument2));
		}

		[Test]
		public void ChangingAPropertyShoulCallCanExecuteOnDispatcher()
		{
			var dispatcherMock = new Mock<IDispatcher>();
			dispatcherMock.Expect(d => d.CheckAccess()).Returns(true);

			DispatcherManager.Current = dispatcherMock.Object;
			var weaver = new NotifyingObjectWeaver();
			var proxyType = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(proxyType) as MyViewModel;
			var notifyingObject = instance as INotifyPropertyChanged;
			var fired = false;
			notifyingObject.PropertyChanged += (s, e) => fired = true;

			instance.SomeInt = 5;
			dispatcherMock.VerifyAll();
		}

		[Test]
		public void ChangingAPropertyWithoutDispatcherAccessShouldCallBeginInvoke()
		{
			var dispatcher = new Dispatcher();
			dispatcher.CheckAccessReturnValue = false;
			DispatcherManager.Current = dispatcher;
			var weaver = new NotifyingObjectWeaver();
			var proxyType = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(proxyType) as MyViewModel;
			var notifyingObject = instance as INotifyPropertyChanged;
			var fired = false;
			notifyingObject.PropertyChanged += (s, e) => fired = true;

			instance.SomeInt = 5;
			Assert.That(dispatcher.BeginInvokeDelegateCalled, Is.True);
		}

		[Test]
		public void InterfacesImplementedByBaseTypeShouldBePresentInProxy()
		{
			var dispatcher = new Dispatcher();
			DispatcherManager.Current = dispatcher;
			var weaver = new NotifyingObjectWeaver();
			var proxyType = weaver.GetProxyType<TypeImplementingOtherInterface>();

			var interfaceType = proxyType.GetInterface(typeof(ISomeInterface).Name, true);
			Assert.That(interfaceType, Is.Not.Null);
		}

		[Test]
		public void InterfacesFromOtherAssemblyImplementedByBaseTypeShouldBePresentInProxy()
		{
			var dispatcher = new Dispatcher();
			DispatcherManager.Current = dispatcher;
			var weaver = new NotifyingObjectWeaver();
			var proxyType = weaver.GetProxyType<TypeImplementingInterfaceInOtherAssembly>();

			var interfaceType = proxyType.GetInterface(typeof(IDisplay).Name, true);
			Assert.That(interfaceType, Is.Not.Null);
		}

		[Test]
		public void ToStringShouldBeOverriddenAndReturnTheBaseTypeFullNameIfNotOverriddenInBaseTyp()
		{
			var dispatcher = new Dispatcher();
			DispatcherManager.Current = dispatcher;
			var weaver = new NotifyingObjectWeaver();
			var proxyType = weaver.GetProxyType<SimpleObject>();

			var instance = Activator.CreateInstance(proxyType);
			var toStringResult = instance.ToString();

			Assert.That(toStringResult, Is.EqualTo(typeof(SimpleObject).ToString()));
		}

		[Test]
		public void ToStringShouldNotBeOverridenIfBaseTypeOverridesIt()
		{
			var dispatcher = new Dispatcher();
			DispatcherManager.Current = dispatcher;
			var weaver = new NotifyingObjectWeaver();
			var proxyType = weaver.GetProxyType<SimpleObjectOverridingToString>();

			var instance = Activator.CreateInstance(proxyType);
			var toStringResult = instance.ToString();

			Assert.That(toStringResult, Is.EqualTo(SimpleObjectOverridingToString.ToStringReturn));
		}

		[Test]
		public void TypesShouldHaveXmlRootAttributeAdded()
		{
			var dispatcher = new Dispatcher();
			DispatcherManager.Current = dispatcher;
			var weaver = new NotifyingObjectWeaver();

			var proxyType = weaver.GetProxyType<SimpleObject>();
			var hasXmlRootAttribute = false;
			var elementName = string.Empty;
			var attributes = proxyType.GetCustomAttributes(true);
			foreach( var attribute in attributes )
			{
				if( attribute is XmlRootAttribute )
				{
					elementName = ((XmlRootAttribute)attribute).ElementName;
					hasXmlRootAttribute = true;
				}
			}

			Assert.That(hasXmlRootAttribute,Is.True);
			Assert.That(elementName,Is.EqualTo(typeof(SimpleObject).Name));
		}
	}
}