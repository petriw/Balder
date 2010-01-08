using Balder.Core.Execution;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.NodesControl
{
	public class MyGame : Game
	{
		public override void OnInitialize()
		{
			ContentManager.AssetsRoot = "Samples/Data/ItemsControl/Assets";

			base.OnInitialize();
		}
	}
}
