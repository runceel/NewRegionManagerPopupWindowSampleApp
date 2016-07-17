using NewRegionManagerPopupWindowSampleApp.Commons;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace NewRegionManagerPopupWindowSampleApp.ViewModels
{
    public class ChildContentViewAViewModel : BindableBase, IRegionManagerAware
    {
        private IRegionManager regionManager;

        public IRegionManager RegionManager
        {
            get { return this.regionManager; }
            set { this.SetProperty(ref this.regionManager, value); }
        }

        public DelegateCommand NavigateCommand { get; }

        public ChildContentViewAViewModel()
        {
            this.NavigateCommand = new DelegateCommand(() => this.RegionManager.RequestNavigate("Main", "ChildContentViewB"));
        }
    }
}
