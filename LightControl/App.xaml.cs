using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LightControl.Views;
using Prism.Ioc;
using Prism.Regions;

namespace LightControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void OnInitialized()
        {
            Container.Resolve<RegionManager>()
                .RegisterViewWithRegion("MainContent", typeof(Presets))
                .RegisterViewWithRegion("MainContent", typeof(Groups))
                .RegisterViewWithRegion("MainContent", typeof(LampMap));
                
            base.OnInitialized();
        }
    }
}
