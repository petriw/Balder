﻿#region License
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

using System.ComponentModel;
using Balder.Core.Execution;
using Balder.Core.Imaging;
using Balder.Core.TypeConverters;

namespace Balder.Core.Materials
{
	public partial class Material
	{
		public Material()
		{
			Shade = MaterialShade.None;
			Diffuse = Color.Random();
		}


		[TypeConverter(typeof(ColorConverter))]
		public Color Ambient { get; set; }

		[TypeConverter(typeof(ColorConverter))]
		public Color Diffuse { get; set; }

		[TypeConverter(typeof(ColorConverter))]
		public Color Specular { get; set; }
		public float Shine { get; set; }
		public float ShineStrength { get; set; }

		public MaterialShade Shade { get; set; }

		public bool DoubleSided { get; set; }

		public static readonly Property<Material, Image> DiffuseMapProperty =
			Property<Material, Image>.Register(m => m.DiffuseMap);
		public Image DiffuseMap
		{
			get { return DiffuseMapProperty.GetValue(this); }
			set { DiffuseMapProperty.SetValue(this, value); }
		}

		public static readonly Property<Material, Image> ReflectionMapProperty =
			Property<Material, Image>.Register(m => m.ReflectionMap);
		public Image ReflectionMap
		{
			get { return ReflectionMapProperty.GetValue(this); }
			set { ReflectionMapProperty.SetValue(this, value); }
		}
	}
}
