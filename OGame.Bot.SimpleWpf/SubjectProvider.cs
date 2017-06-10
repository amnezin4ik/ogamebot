using Autofac;
using AutoMapper;
using NLog;
using NLog.Config;
using NLog.Targets;
using OGame.Bot.Application;
using OGame.Bot.Domain.Services;
using OGame.Bot.Infrastructure.API;
using OGame.Bot.Modules.Common;

namespace OGame.Bot.SimpleWpf
{
    public class SubjectProvider
    {
        private readonly IContainer _container;

        public SubjectProvider()
        {
            var builder = new ContainerBuilder();
            ConfigureContainer(builder);
            _container = builder.Build();
        }

        public T Create<T>()
        {
            return _container.Resolve<T>();
        }

        public T Create<T>(string propertyName, object value)
        {
            return _container.Resolve<T>(new NamedParameter(propertyName, value));
        }

        public T Create<T>(string propertyName1, object value1, string propertyName2, object value2)
        {
            return _container.Resolve<T>(new NamedParameter(propertyName1, value1), new NamedParameter(propertyName2, value2));
        }

        private void ConfigureContainer(ContainerBuilder builder)
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
                cfg.AddProfile(typeof(WpfMappingProfile));
                cfg.AddProfile(typeof(ApplicationMappingProfile));
                cfg.AddProfile(typeof(DomainMappingProfile));
            }));
            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>();
        }

        private void ConfigureLogging()
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

            LogManager.Configuration = config;
        }
    }
}