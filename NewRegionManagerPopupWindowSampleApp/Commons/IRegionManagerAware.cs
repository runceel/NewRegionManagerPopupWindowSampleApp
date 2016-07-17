using Prism.Regions;

namespace NewRegionManagerPopupWindowSampleApp.Commons
{
    public interface IRegionManagerAware
    {
        IRegionManager RegionManager { get; set; }
    }
}
