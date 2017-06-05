using System.Windows;
using Autofac;
using AutoMapper;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using OGame.Bot.Application;
using OGame.Bot.Domain.Services;
using OGame.Bot.Infrastructure.API;
using OGame.Bot.Modules.Common;
using OGame.Bot.Wpf.ViewModels;

namespace OGame.Bot.Wpf
{
    public class AppBootstrapper : AutofacBootstrapper<MainWindowViewModel>
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<WpfDiModule>();
            builder.RegisterModule<ApplicationDiModule>();
            builder.RegisterModule<InfrastructureDiModule>();
            builder.RegisterModule<DomainDiModule>();
            builder.RegisterModule<ModulesCommonDiModule>();
            RegisterAutomapperConfiguration(builder);
        }

        private void RegisterAutomapperConfiguration(ContainerBuilder builder)
        {
            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                cfg.ConstructServicesUsing(Container.Resolve);
                cfg.AddProfile(typeof(WpfMappingProfile));
                cfg.AddProfile(typeof(ApplicationMappingProfile));
                cfg.AddProfile(typeof(DomainMappingProfile));
            }));
            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>();
        }

        protected override void ConfigureBootstrapper()
        {
            base.ConfigureBootstrapper();
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = "OGame.Bot.Wpf.Views",
                DefaultSubNamespaceForViewModels = "OGame.Bot.Wpf.ViewModels"
            };
            ViewLocator.ConfigureTypeMappings(config);
            ViewModelLocator.ConfigureTypeMappings(config);
        }
    }
}
