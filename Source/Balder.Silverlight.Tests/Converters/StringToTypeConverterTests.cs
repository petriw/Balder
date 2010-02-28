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
using System.Globalization;
using Balder.Silverlight.Converters;
using NUnit.Framework;

namespace Balder.Silverlight.Tests.Converters
{
	[TestFixture]
	public class StringToTypeConverterTests
	{
		[Test]
		public void UsingValidTypeStringShouldReturnTheTypeRepresentedInString()
		{
			var typeString = typeof (StringToTypeConverterTests).AssemblyQualifiedName;
			var converter = new StringToTypeConverter();
			var type = converter.ConvertFrom(null, CultureInfo.InvariantCulture, typeString) as Type;
			Assert.That(type,Is.Not.Null);
			Assert.That(type.AssemblyQualifiedName, Is.EqualTo(typeString));
		}

		[Test]
		public void UsingInvalidTypeStringShouldReturnNull()
		{
			var typeString = Guid.NewGuid().ToString();
			var converter = new StringToTypeConverter();
			var type = converter.ConvertFrom(null, CultureInfo.InvariantCulture, typeString) as Type;
			Assert.That(type,Is.Null);
		}
		
		[Test]
		public void UsingEmtpyStringShouldReturnNull()
		{
			var typeString = string.Empty;
			var converter = new StringToTypeConverter();
			var type = converter.ConvertFrom(null, CultureInfo.InvariantCulture, typeString) as Type;
			Assert.That(type, Is.Null);
 		}
	}
}
