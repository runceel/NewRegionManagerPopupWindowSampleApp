using NewRegionManagerPopupWindowSampleApp.Commons;
using Prism.Mvvm;
using Prism.Regions;

namespace NewRegionManagerPopupWindowSampleApp.ViewModels
{
    public class ChildViewAViewModel : BindableBase, IRegionManagerAware
    {
        private IRegionManager regionManager;

        public IRegionManager RegionManager
        {
            get { return this.regionManager; }
            set { this.SetProperty(ref this.regionManager, value); }
        }

        public void Initialize()
        {
            this.RegionManager.RequestNavigate("Main", "ChildContentViewA");
        }
    }
}
