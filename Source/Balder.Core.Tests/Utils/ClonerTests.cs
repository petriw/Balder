#if(!SILVERLIGHT)
using System;
using System.Runtime.Serialization;
#endif
using System.Collections;
using System.Linq;
using System.Xml.Serialization;
using Balder.Core.Utils;
using NUnit.Framework;
using System.Collections.Generic;

namespace Balder.Core.Tests.Utils
{
	[TestFixture]
	public class ClonerTests
	{
		#region Stubs
		public class SimpleObject
		{ }

		public class ObjectWithFields
		{
			public int SomeValue;
			public string SomeString;
			public double SomeDouble;
		}

		public class ObjectWithProperties
		{
			public int SomeValue { get; set; }
			public string SomeString { get; set; }
			public double SomeDouble { get; set; }
		}

		public class ObjectWithReferenceAsField
		{
			public ObjectWithFields Reference;
		}

		public class ObjectWithReferenceAsProperty
		{
			public ObjectWithProperties Reference { get; set; }
		}

		public class ObjectWithListAsField
		{
			public ObjectWithListAsField()
			{
				Objects = new List<ObjectWithFields>();
			}

			public List<ObjectWithFields> Objects;
		}

		public class ObjectWithListAsProperty
		{
			public ObjectWithListAsProperty()
			{
				Objects = new List<ObjectWithProperties>();
			}

			public List<ObjectWithProperties> Objects { get; set; }
		}

		public class ObjectWithEnumerableAsProperty
		{
			public ObjectWithEnumerableAsProperty()
			{
				Objects = new List<ObjectWithFields>
				          	{
				          		new ObjectWithFields
				          			{
				          				SomeValue = 5,
				          				SomeString = "Hello",
				          				SomeDouble = 0.5
				          			},
				          		new ObjectWithFields
				          			{
				          				SomeValue = 6,
				          				SomeString = "World",
				          				SomeDouble = 1
				          			},
				          	};
			}

			public IEnumerable<ObjectWithFields> Objects { get; set; }
		}

		public class ObjectWithPrivateSetterProperty
		{
			public void Initialize()
			{
				SomeValue = 5;
			}


			public int SomeValue { get; private set; }
		}

		public class ObjectWithPropertyMarkedXmlIgnore
		{
			public void Initialize()
			{
				SomeValue = 5;
			}

			[XmlIgnore]
			public int SomeValue { get; private set; }
		}

#if(!SILVERLIGHT)
		public class ObjectWithPropertyMarkedIgnoreDataMember
		{
			public void Initialize()
			{
				SomeValue = 5;
			}

			[IgnoreDataMember]
			public int SomeValue { get; private set; }
		}
#endif

		public class ObjectWithParentReference
		{
			public ObjectWithChildReference Parent { get; set; }
		}

		public class ObjectWithChildReference
		{
			public ObjectWithParentReference Child { get; set; }
		}

		public class ObjectWithCollectionAsPropertyReturningArray
		{
			public ObjectWithCollectionAsPropertyReturningArray()
			{
				SomeCollection = new ObjectWithProperties[]
				                 	{
				                 		new ObjectWithProperties
				                 			{
				                 				SomeValue = 5,
				                 				SomeString = "Hello",
				                 				SomeDouble = 0.5
				                 			},
				                 		new ObjectWithProperties
				                 			{
				                 				SomeValue = 6,
				                 				SomeString = "World",
				                 				SomeDouble = 1
				                 			},
				                 	};

			}

			public ICollection SomeCollection { get; set; }
		}


		public class ObjectWithGenericCollectionAsPropertyReturningArray
		{
			public ObjectWithGenericCollectionAsPropertyReturningArray()
			{
				SomeCollection = new ObjectWithProperties[]
				                 	{
				                 		new ObjectWithProperties
				                 			{
				                 				SomeValue = 5,
				                 				SomeString = "Hello",
				                 				SomeDouble = 0.5
				                 			},
				                 		new ObjectWithProperties
				                 			{
				                 				SomeValue = 6,
				                 				SomeString = "World",
				                 				SomeDouble = 1
				                 			},
				                 	};

			}

			public ICollection<ObjectWithProperties> SomeCollection { get; set; }
		}

		public class ObjectWithArrayOfFloatsAsProperty
		{
			public ObjectWithArrayOfFloatsAsProperty()
			{
				Floats = new float[10];

				for (var index = 0; index < Floats.Length; index++)
				{
					Floats[index] = (float) index;
				}
			}

			public float[] Floats { get; set; }
		}


		#endregion

		[Test]
		public void CloningAnObjectShouldHaveDifferentHashCode()
		{
			var obj = new SimpleObject();
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.GetHashCode(), Is.Not.EqualTo(obj.GetHashCode()));
		}

		[Test]
		public void CloningAnObjectShouldCloneItsFields()
		{
			var obj = new ObjectWithFields
						{
							SomeValue = 5,
							SomeString = "Hello",
							SomeDouble = 0.5
						};
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.SomeValue, Is.EqualTo(obj.SomeValue));
			Assert.That(clone.SomeString, Is.EqualTo(obj.SomeString));
			Assert.That(clone.SomeDouble, Is.EqualTo(obj.SomeDouble));
		}

		[Test]
		public void CloningAnObjectShouldCloneItsProperties()
		{
			var obj = new ObjectWithProperties
						{
							SomeValue = 5,
							SomeString = "Hello",
							SomeDouble = 0.5
						};
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.SomeValue, Is.EqualTo(obj.SomeValue));
			Assert.That(clone.SomeString, Is.EqualTo(obj.SomeString));
			Assert.That(clone.SomeDouble, Is.EqualTo(obj.SomeDouble));
		}

		[Test]
		public void CloningAnObjectWithReferenceAsFieldShouldCloneReferenceIntoNewObject()
		{
			var obj = new ObjectWithReferenceAsField
						{
							Reference = new ObjectWithFields
											{
												SomeValue = 5,
												SomeString = "Hello",
												SomeDouble = 0.5
											}
						};
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.Reference.GetHashCode(), Is.Not.EqualTo(obj.Reference.GetHashCode()));
			Assert.That(clone.Reference.SomeValue, Is.EqualTo(obj.Reference.SomeValue));
			Assert.That(clone.Reference.SomeString, Is.EqualTo(obj.Reference.SomeString));
			Assert.That(clone.Reference.SomeDouble, Is.EqualTo(obj.Reference.SomeDouble));
		}

		[Test]
		public void CloningAnObjectWithReferenceAsPropertiesShouldCloneReferenceIntoNewObject()
		{
			var obj = new ObjectWithReferenceAsProperty
						{
							Reference = new ObjectWithProperties
											{
												SomeValue = 5,
												SomeString = "Hello",
												SomeDouble = 0.5
											}
						};
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.Reference.GetHashCode(), Is.Not.EqualTo(obj.Reference.GetHashCode()));
			Assert.That(clone.Reference.SomeValue, Is.EqualTo(obj.Reference.SomeValue));
			Assert.That(clone.Reference.SomeString, Is.EqualTo(obj.Reference.SomeString));
			Assert.That(clone.Reference.SomeDouble, Is.EqualTo(obj.Reference.SomeDouble));
		}

		[Test]
		public void CloningAnObjectWithListAsFieldShouldCloneAllItemsInList()
		{
			var obj = new ObjectWithListAsField
						{
							Objects = new List<ObjectWithFields>
			          		          	{
			          		          		new ObjectWithFields
			          		          			{
			          		          				SomeValue = 5,
			          		          				SomeString = "Hello",
			          		          				SomeDouble = 0.5
			          		          			},
			          		          		new ObjectWithFields
			          		          			{
			          		          				SomeValue = 6,
			          		          				SomeString = "World",
			          		          				SomeDouble = 1
			          		          			},
			          		          	}
						};
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.Objects.Count, Is.EqualTo(obj.Objects.Count));
			for (var index = 0; index < obj.Objects.Count; index++)
			{
				var originalChild = obj.Objects[index];
				var clonedChild = clone.Objects[index];
				Assert.That(clonedChild.GetHashCode(), Is.Not.EqualTo(originalChild.GetHashCode()));
				Assert.That(clonedChild.SomeValue, Is.EqualTo(originalChild.SomeValue));
				Assert.That(clonedChild.SomeString, Is.EqualTo(originalChild.SomeString));
				Assert.That(clonedChild.SomeDouble, Is.EqualTo(originalChild.SomeDouble));
			}
		}

		[Test]
		public void CloningAnObjectWithListAsPropertyShouldCloneAllItemsInList()
		{
			var obj = new ObjectWithListAsProperty
						{
							Objects = new List<ObjectWithProperties>
			          		          	{
			          		          		new ObjectWithProperties
			          		          			{
			          		          				SomeValue = 5,
			          		          				SomeString = "Hello",
			          		          				SomeDouble = 0.5
			          		          			},
			          		          		new ObjectWithProperties
			          		          			{
			          		          				SomeValue = 6,
			          		          				SomeString = "World",
			          		          				SomeDouble = 1
			          		          			},
			          		          	}
						};
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.Objects.Count, Is.EqualTo(obj.Objects.Count));
			for (var index = 0; index < obj.Objects.Count; index++)
			{
				var originalChild = obj.Objects[index];
				var clonedChild = clone.Objects[index];
				Assert.That(clonedChild.GetHashCode(), Is.Not.EqualTo(originalChild.GetHashCode()));
				Assert.That(clonedChild.SomeValue, Is.EqualTo(originalChild.SomeValue));
				Assert.That(clonedChild.SomeString, Is.EqualTo(originalChild.SomeString));
				Assert.That(clonedChild.SomeDouble, Is.EqualTo(originalChild.SomeDouble));
			}
		}

		[Test]
		public void CloningAnObjectWithEnumerableShouldCloneAllItemsInList()
		{
			var obj = new ObjectWithEnumerableAsProperty();
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.Objects.Count(), Is.EqualTo(obj.Objects.Count()));
			var originalObjects = obj.Objects.ToArray();
			var clonedObjects = clone.Objects.ToArray();

			for (var index = 0; index < obj.Objects.Count(); index++)
			{
				var originalChild = originalObjects[index];
				var clonedChild = clonedObjects[index];
				Assert.That(clonedChild.GetHashCode(), Is.Not.EqualTo(originalChild.GetHashCode()));
				Assert.That(clonedChild.SomeValue, Is.EqualTo(originalChild.SomeValue));
				Assert.That(clonedChild.SomeString, Is.EqualTo(originalChild.SomeString));
				Assert.That(clonedChild.SomeDouble, Is.EqualTo(originalChild.SomeDouble));
			}
		}

		[Test]
		public void CloningAnObjectWithPrivateSetterPropertyShouldCloneProperties()
		{
			var obj = new ObjectWithPrivateSetterProperty();
			obj.Initialize();
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.SomeValue, Is.EqualTo(obj.SomeValue));
		}

		[Test]
		public void CloningAnObjectWithPropertyMarkedWithXmlIgnoreShouldNotCloneProperty()
		{
			var obj = new ObjectWithPropertyMarkedXmlIgnore();
			obj.Initialize();
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.SomeValue, Is.EqualTo(0));
		}

		[Test]
		public void CloningAnObjectWithPropertyMarkedWithIgnoreDataMemberShouldNotCloneProperty()
		{
			var obj = new ObjectWithPropertyMarkedXmlIgnore();
			obj.Initialize();
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.SomeValue, Is.EqualTo(0));
		}

		[Test]
		public void CloningObjectWithCyclicReferencesShouldNotCauseStackOverflowException()
		{
			var obj = new ObjectWithChildReference
						{
							Child = new ObjectWithParentReference()
						};
			obj.Child.Parent = obj;
			Cloner.DeepClone(obj);
		}

		[Test]
		public void CloningObjectWithArrayReturnedAsCollectionShouldRecreateArray()
		{
			var obj = new ObjectWithCollectionAsPropertyReturningArray();
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.SomeCollection.Count, Is.EqualTo(obj.SomeCollection.Count));
		}

		[Test]
		public void CloningObjectWithArrayReturnedAsGenericCollectionShouldRecreateArray()
		{
			var obj = new ObjectWithGenericCollectionAsPropertyReturningArray();
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.SomeCollection.Count, Is.EqualTo(obj.SomeCollection.Count));
		}

		[Test]
		public void CloningAnArrayOfValuesShouldCloneTheArrayAsArrayAndCopyTheContent()
		{
			var obj = new ObjectWithArrayOfFloatsAsProperty();
			var clone = Cloner.DeepClone(obj);
			Assert.That(clone.Floats, Is.TypeOf(typeof(float[])));
			Assert.That(clone.Floats.Length,Is.EqualTo(obj.Floats.Length));

			for( var index=0; index<obj.Floats.Length; index++ )
			{
				Assert.That(clone.Floats[index],Is.EqualTo(obj.Floats[index]));
			}
		}
	}
}