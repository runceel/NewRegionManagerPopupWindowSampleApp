using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System.Linq;

namespace NewRegionManagerPopupWindowSampleApp
{
    class AppModule : IModule
    {
        private IUnityContainer Container { get; }

        private IRegionManager RegionManager { get; }

        public AppModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.Container = container;
            this.RegionManager = regionManager;
        }

        public void Initialize()
        {
            // register views
            this.Container.RegisterTypes(
                AllClasses.FromAssemblies(typeof(AppModule).Assembly)
                    .Where(x => x.Namespace == "NewRegionManagerPopupWindowSampleApp.Views"),
                _ => new[] { typeof(object) },
                WithName.TypeName,
                WithLifetime.PerResolve);

            this.RegionManager.RequestNavigate("Main", "ViewA");
        }
    }
}
