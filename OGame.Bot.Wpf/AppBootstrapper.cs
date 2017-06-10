﻿using System.Reflection;
using System.Windows;
using Autofac;
using AutoMapper;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using NLog;
using NLog.Config;
using NLog.Targets;
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
            ConfigureLogging();
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

        public void ConfigureLogging()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            var logTarget = new MemoryEventTarget();
            config.AddTarget("memory", logTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            
            fileTarget.FileName = "${basedir}/log123.txt";
            fileTarget.Layout = @"${date:format=HH\:mm\:ss} ${level:uppercase=true} ${logger} ${message} ${exception:format=toString}";

            var rule1 = new LoggingRule("*", LogLevel.Debug, logTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            NLog.LogManager.Configuration = config;
        }
    }
}
