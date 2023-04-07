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
