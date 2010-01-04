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
		public class DeviceContextedNode : Node, IDeviceContext
		{
			private Guid _context;

			public DeviceContextedNode()
			{
				_context = Guid.NewGuid();
			}

			public object GetContext()
			{
				return _context.ToString();
			}

			public int SomeProperty { get; set; }
		}

		private static IObjectFactory GetObjectFactory()
		{
			var objectFactory = new ObjectFactoryStub();
			return objectFactory;
		}


		[Test, SilverlightUnitTest]
		public void CloningShouldReturnNewObject()
		{
			var objectFactory = GetObjectFactory();
			var node = new DeviceContextedNode();
			var contentCreator = new ContentCreator(objectFactory);

			var clone = contentCreator.Clone(node);
			Assert.That(clone,Is.Not.Null);
			Assert.That(clone,Is.Not.EqualTo(node));
		}

		[Test, SilverlightUnitTest]
		public void CloningWithDeviceContextAndContentIsReferenceShouldHoldSameReference()
		{
			var objectFactory = GetObjectFactory();
			var node = new DeviceContextedNode();
			var nodeContext = node.GetContext();
			var contentCreator = new ContentCreator(objectFactory);

			var clone = contentCreator.Clone(node);
			var cloneContext = clone.GetContext();

			Assert.That(cloneContext, Is.EqualTo(nodeContext));
		}

		[Test, SilverlightUnitTest]
		public void CloningShouldMemberwizeCloneAllMembers()
		{
			var objectFactory = GetObjectFactory();
			var node = new DeviceContextedNode();
			node.SomeProperty = 42;
			var contentCreator = new ContentCreator(objectFactory);

			var clone = contentCreator.Clone(node);
			Assert.That(clone.SomeProperty, Is.EqualTo(node.SomeProperty));
		}
	}
}

