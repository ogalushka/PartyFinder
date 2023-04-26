using Autofac;
using Autofac.Core;
using System.Windows.Input;

namespace WPFClient.Factory
{
    public class CommandFactory
    {
        private readonly ILifetimeScope scope;

        public CommandFactory(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        // TODO a bit off that we register view models and also expect them to be passed as params
        // can be a pain to find if you forget to pass view model and get injected with a new one from DI conainer
        public TCommand Get<TCommand>(params object[] param) where TCommand : ICommand
        {
            var typedParameters = new Parameter[param.Length];
            for (var i = 0; i < param.Length; i++)
            {
                typedParameters[i] = new TypedParameter(param[i].GetType(), param[i]);
            }
            return scope.Resolve<TCommand>(typedParameters);
        }
    }
}
