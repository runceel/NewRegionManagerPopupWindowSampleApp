using Microsoft.Practices.Unity;
using NewRegionManagerPopupWindowSampleApp.Commons;
using NewRegionManagerPopupWindowSampleApp.Views;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using System.Windows;

namespace NewRegionManagerPopupWindowSampleApp
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            var mc = (ModuleCatalog)this.ModuleCatalog;
            mc.AddModule(typeof(AppModule));
        }

        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var f = base.ConfigureDefaultRegionBehaviors();
            f.AddIfMissing(RegionManagerAwareBehavior.Key, typeof(RegionManagerAwareBehavior));
            return f;
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
