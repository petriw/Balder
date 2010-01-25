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

namespace Balder.Core.Execution
{
	///<summary>
	/// Adds composition to any component/class
	/// 
	/// The purpose is to provide a generic way of importing instances of types
	/// from any loaded assembly based on [Import] criterias set
	///</summary>
	public interface IComposer
	{
		/// <summary>
		/// Satisfy all imports for an instance of a component
		/// </summary>
		/// <param name="component">Component to satisfy imports for</param>
		void SatisfyImportsFor(object component);
	}
}