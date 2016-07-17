using Microsoft.Practices.ServiceLocation;
using Prism.Common;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Interactivity;

namespace NewRegionManagerPopupWindowSampleApp.Commons
{
    public class CreateNewRegionManagerBehavior : Behavior<DependencyObject>
    {
        protected override void OnAttached()
        {
            var rm = ServiceLocator.Current.GetInstance<IRegionManager>();
            var newRegionManager = rm.CreateRegionManager();
            RegionManager.SetRegionManager(this.AssociatedObject, newRegionManager);
            Action<IRegionManagerAware> setRegionManager = x => x.RegionManager = newRegionManager;
            MvvmHelpers.ViewAndViewModelAction(this.AssociatedObject, setRegionManager);
        }

        protected override void OnDetaching()
        {
            RegionManager.SetRegionManager(this.AssociatedObject, null);
            Action<IRegionManagerAware> resetRegionManager = x => x.RegionManager = null;
            MvvmHelpers.ViewAndViewModelAction(this.AssociatedObject, resetRegionManager);
        }
    }
}
