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
namespace Balder.Core
{
	public struct ColorSpace
	{
		public int RedPosition;
		public int GreenPosition;
		public int BluePosition;
		public int AlphaPosition;


		/// <summary>
		/// Gets or sets wether or not the position of the components are in bytes 
		/// or in bits within the depth of the colorspace.
		/// 
		/// The depth would typically be a dword/long word (32 bits) for 32 bits per pixel or a word for 15/16 bits per
		/// pixel
		/// </summary>
		public bool BytePositions;
	}
}