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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Balder.Core.Objects.Geometries;
using Balder.Silverlight.Controls;
using CThru.Silverlight;
using NUnit.Framework;

namespace Balder.Silverlight.Tests.Controls
{
	[TestFixture]
	public class NodesControlTests
	{
		private const string NodeTemplateXaml =
			"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" + // http://schemas.microsoft.com/winfx/2006/xaml/presentation
            "<ComboBox/>"+
			//"xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" "+
			//"xmlns:Geometries=\"clr-namespace:Balder.Core.Objects.Geometries;assembly=Balder.Core\">"+
			"</DataTemplate>";


		private DataTemplate Create(Type type)
		{
			var dt = new DataTemplate();
			string xaml = @"<DataTemplate 
                xmlns=""http://schemas.microsoft.com/client/2007""
                xmlns:controls=""clr-namespace:" + type.Namespace + @";assembly=" + type.Namespace + @""">
                <controls:" + type.Name + @"/></DataTemplate>";
			return (DataTemplate)XamlReader.Load(xaml);
		}
		[Test, SilverlightUnitTest]
		public void SettingDataSourceWithANodeTemplateShouldGenerateAllChildren()
		{
			var nodesControl = new NodesControl();

			var nodeTemplate = Create(typeof (Mesh));
				//XamlReader.Load(NodeTemplateXaml);

			//nodesControl.NodeTemplate = nodeTemplate;
			
			throw new NotImplementedException();
		}

		[Test, SilverlightUnitTest]
		public void ChildrenCreatedBasedOnNodeTemplateShouldCreateInstancesOfContentOfNodeTemplate()
		{
			throw new NotImplementedException();
		}


		[Test, SilverlightUnitTest]
		public void AddingElementsToAnItemsSourceImplementingINotifyCollectionChangedShouldAddChildDynamically()
		{
			throw new NotImplementedException();
		}

		[Test, SilverlightUnitTest]
		public void RemovingElementFromAnItemsSourceImplementingINotifyCollectionChangedShouldRemoveChildDynamically()
		{
			throw new NotImplementedException();
		}

		[Test, SilverlightUnitTest]
		public void ChildrenCreatedBasedOnTemplateShouldHaveTheirDataContextSetToItemsInItemsSource()
		{
			throw new NotImplementedException();
		}
	}
}

