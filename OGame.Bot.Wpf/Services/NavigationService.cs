using System;
using System.Collections.Generic;
using Autofac;
using Caliburn.Micro;

namespace OGame.Bot.Wpf.Services
{
    public class NavigationService : INavigationService
    {
        private readonly ILifetimeScope _diContainer;
        private IConductor _conductor;

        public NavigationService(ILifetimeScope diContainer)
        {
            _diContainer = diContainer;
        }

        public void Initialize(IConductor conductor)
        {
            _conductor = conductor;
        }

        public void Navigate<T>(params object[] parameters) where T : IScreen
        {
            if (_conductor == null)
            {
                throw new InvalidOperationException("You should initialize this service before use it");
            }

            List<global::Autofac.Core.Parameter> typedParameters = new List<global::Autofac.Core.Parameter>();
            foreach (var parameter in parameters)
            {
                var typedParameter = new TypedParameter(parameter.GetType(), parameter);
                typedParameters.Add(typedParameter);
            }

            var screen = (IScreen)_diContainer.Resolve(typeof(T), typedParameters);
            _conductor.ActivateItem(screen);
        }
    }
}