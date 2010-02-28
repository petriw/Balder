using Ninject.Core;

namespace Balder.Silverlight.SampleBrowser
{
	public class ViewModels
	{
		public ViewModels()
		{
			App.Kernel.Inject(this);
		}

		[Inject]
		public Features.Resources.ViewModel Resources { get; set; }
	}
}
