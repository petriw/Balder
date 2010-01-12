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

namespace Balder.Core.SoftwareRendering
{
	public class BufferContainer
	{
		public static int[] Framebuffer { get; set; }
		public static UInt32[] DepthBuffer { get; set; }

		public static int Width { get; set; }
		public static int Height { get; set; }
		public static int Stride { get; set; }

		public static int RedPosition { get; set; }
		public static int GreenPosition { get; set; }
		public static int BluePosition { get; set; }
		public static int AlphaPosition { get; set; }
	}
}
