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
using Balder.Core.Content;
using Balder.Core.Execution;
using Balder.Core.Tests.Stubs;
using CThru.Silverlight;
using NUnit.Framework;

namespace Balder.Core.Tests.Content
{
	[TestFixture]
	public class ContentCreatorTests
	{
		public class DeviceContextNode : Node
		{
			public DeviceContextNode()
			{
				Context = Guid.NewGuid();
			}

			public Guid Context { get; set; }

			public int SomeProperty { get; set; }


			public bool CopyFromCalled = false;
			public override void CopyFrom(Node source)
			{
				CopyFromCalled = true;
				SomeProperty = ((DeviceContextNode) source).SomeProperty;
				Context = ((DeviceContextNode)source).Context;
				base.CopyFrom(source);
			}
		}

		private static IObjectFactory GetObjectFactory()
		{
			var objectFactory = new ObjectFactoryStub();
			return objectFactory;
		}


		[Test, SilverlightUnitTest]
		public void ReferenceCopyingShouldReturnNewObject()
		{
			var objectFactory = GetObjectFactory();
			var node = new DeviceContextNode();
			var contentCreator = new ContentCreator(objectFactory);

			var clone = contentCreator.ReferenceCopy(node);
			Assert.That(clone,Is.Not.Null);
			Assert.That(clone,Is.Not.EqualTo(node));
		}


		[Test, SilverlightUnitTest]
		public void ReferenceCopyingShouldCallCopyFrom()
		{
			var objectFactory = GetObjectFactory();
			var node = new DeviceContextNode();
			node.SomeProperty = 42;
			var contentCreator = new ContentCreator(objectFactory);

			var clone = contentCreator.ReferenceCopy(node);
			Assert.That(clone.CopyFromCalled, Is.True);
			Assert.That(clone.SomeProperty, Is.EqualTo(node.SomeProperty));
		}

		[Test, SilverlightUnitTest]
		public void ReferenceCopyingWithChildrenShouldCloneChildren()
		{
			var objectFactory = GetObjectFactory();
			var parent = new DeviceContextNode();
			var child = new DeviceContextNode();
			parent.Children.Add(child);
			var contentCreator = new ContentCreator(objectFactory);

			var clone = contentCreator.ReferenceCopy(parent);
			Assert.That(clone.Children.Count, Is.EqualTo(parent.Children.Count));
		}
	}
}

