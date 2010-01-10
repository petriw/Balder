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
using System.ComponentModel;
using Balder.Silverlight.Notification;
using NUnit.Framework;

namespace Balder.Silverlight.Tests.Notification
{
	[TestFixture]
	public class NotifyingObjectWeaverTests
	{
		#region Stubs
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

		#endregion

		[Test]
		public void CreatedTypeShouldInheritFromTypeGiven()
		{
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			Assert.That(type.BaseType, Is.EqualTo(typeof (MyViewModel)));
		}

		[Test]
		public void CreatedTypeShouldImplementINotifyPropertyChanged()
		{
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var interfaceType = type.GetInterface(typeof (INotifyPropertyChanged).Name, true);
			Assert.That(interfaceType,Is.Not.Null);
		}

		[Test]
		public void CallingGeneratedOnPropertyChangedMethodShouldFirePropertyChangedEvent()
		{
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var fired = false;
			instance.PropertyChanged += (s, e) => fired = true;
			var method = type.GetMethod("OnPropertyChanged");
			method.Invoke(instance, new[] {"Something"});
			Assert.That(fired,Is.True);
		}

		[Test]
		public void SettingIntegerPropertyShouldReturnSameValueWhenGetting()
		{
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var myViewModel = instance as MyViewModel;
			var expected = 5;
			myViewModel.SomeInt = expected;
			var actual = myViewModel.SomeInt;
			Assert.That(actual,Is.EqualTo(expected));
		}

		[Test]
		public void SettingIntegerPropertyShouldFirePropertyChangedEvent()
		{
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var myViewModel = instance as MyViewModel;
			var fired = false;
			instance.PropertyChanged += (s, e) =>
			                            	{
			                            		if( e.PropertyName.Equals("SomeInt"))
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
			var weaver = new NotifyingObjectWeaver();
			var type = weaver.GetProxyType<MyViewModel>();
			var instance = Activator.CreateInstance(type) as INotifyPropertyChanged;
			var myViewModel = instance as MyViewModel;
			myViewModel.SomeStringWithImplementation = "Something";
			Assert.That(myViewModel.SomeStringWithImplementationSetCalled,Is.True);
		}

		[Test]
		public void GettingPropertyShouldCallBaseClassGetter()
		{
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
			Assert.That(fired,Is.False);
		}

		[Test]
		public void PropertyDecoratedWithNotificationForMorePropertiesShouldFirePropertyChangedEventForAllProperties()
		{
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
			                            		if( e.PropertyName.Equals("SomeString"))
			                            		{
			                            			someStringFired = true;
			                            		}
			                            	};
			myViewModel.SomeOtherString = "Hello";
			Assert.That(someOtherStringFired,Is.True);
			Assert.That(someStringFired, Is.True);
		}

		[Test]
		public void PropertyWithPrivateSetterShouldBeIgnored()
		{
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
			Assert.That(fired,Is.False);
		}

		[Test]
		public void GettingProxyForSameTypeTwiceShouldYieldSameProxyType()
		{
			var weaver = new NotifyingObjectWeaver();
			var firstType = weaver.GetProxyType<MyViewModel>();
			var secondType = weaver.GetProxyType<MyViewModel>();
			Assert.That(firstType,Is.EqualTo(secondType));
		}

		[Test]
		public void ObjectWithNonDefaultConstructorShouldHaveItsArgumentsForwardedFromProxy()
		{
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
	}
}