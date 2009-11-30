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
using System.Linq;

namespace Balder.Core.Utils
{
	public static class EnumHelper
	{
		public static T[] GetValues<T>()
		{
			var enumType = typeof(T);

			if (!enumType.IsEnum)
			{
				throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
			}

			var values = new List<T>();

			var fields = from field in enumType.GetFields()
			             where field.IsLiteral
			             select field;

			foreach (var field in fields)
			{
				var value = field.GetValue(enumType);
				values.Add((T)value);
			}

			return values.ToArray();
		}

		public static object[] GetValues(Type enumType)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
			}

			var values = new List<object>();

			var fields = from field in enumType.GetFields()
			             where field.IsLiteral
			             select field;

			foreach (var field in fields)
			{
				var value = field.GetValue(enumType);
				values.Add(value);
			}

			return values.ToArray();
		}
	}
}