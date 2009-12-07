using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using Balder.Core.Execution;
using CThru.Silverlight;
using NUnit.Framework;

namespace Balder.Core.Tests.Execution
{
	[TestFixture]
	public class PropertyTests
	{
		public partial class ControlStub
		{
			
		}

		public partial class ControlStub : Control,INotifyPropertyChanged, ICanNotifyChanges
		{
			public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
			public static readonly Property<ControlStub, string> SomeStringProperty = Property<ControlStub, string>.Register(c => c.SomeString);
			public string SomeString { get; set; }

			public void Notify(string propertyName, object oldValue, object newValue)
			{
				PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
			}
		}


		[Test, SilverlightUnitTest]
		public void SettingValueShouldSetItInDependencyObjectPropertyBag()
		{
			var control = new ControlStub();
			var expected = "Something";
			ControlStub.SomeStringProperty.SetValue(control,expected);

			var actual = control.GetValue(ControlStub.SomeStringProperty.ActualDependencyProperty);
			Assert.That(actual,Is.EqualTo(expected));
		}

		[Test, SilverlightUnitTest]
		public void SettingValueShouldFirePropertyChangedEvent()
		{
			var control = new ControlStub();
			var eventFired = false;
			control.PropertyChanged += (s, e) => eventFired = true;

			ControlStub.SomeStringProperty.SetValue(control,"Something");
			Assert.That(eventFired,Is.True);
		}

		[Test,SilverlightUnitTest]
		public void SettingSameValueTwiceShouldFirePropertyChangedEventOnce()
		{
			var control = new ControlStub();
			var eventFireCount = 0;
			control.PropertyChanged += (s, e) => eventFireCount++;

			var valueToSet = "Something";
			ControlStub.SomeStringProperty.SetValue(control, valueToSet);
			ControlStub.SomeStringProperty.SetValue(control, valueToSet);
			Assert.That(eventFireCount, Is.EqualTo(1));
		}

		[Test,SilverlightUnitTest]
		public void SettingValueToNullAfterIthasBeenSetToSomethingElseShouldFirePropertyChangedEvent()
		{
			var control = new ControlStub();
			var eventFireCount = 0;

			var valueToSet = "Something";
			ControlStub.SomeStringProperty.SetValue(control, valueToSet);
			control.PropertyChanged += (s, e) => eventFireCount++;
			ControlStub.SomeStringProperty.SetValue(control, null);
			Assert.That(eventFireCount, Is.EqualTo(1));
		}

		[Test, SilverlightUnitTest]
		public void GettingValueAfterSettingShouldReturnSameValue()
		{
			var control = new ControlStub();
			var expected = "Something";
			ControlStub.SomeStringProperty.SetValue(control, expected);
			var actual = ControlStub.SomeStringProperty.GetValue(control);
			Assert.That(actual,Is.EqualTo(expected));
		}

		[Test, SilverlightUnitTest]
		public void SettingValueInOtherThreadShouldSetValue()
		{
			var control = new ControlStub();
			var expected = "Something";
			var waitEvent = new ManualResetEvent(false);
			var thread = new Thread(() =>
			                        	{
			                        		ControlStub.SomeStringProperty.SetValue(control, expected);
			                        		waitEvent.Set();
			                        	});
			thread.Start();

			waitEvent.WaitOne();
			var actual = ControlStub.SomeStringProperty.GetValue(control);
			Assert.That(actual,Is.EqualTo(expected));
		}
	}
}
