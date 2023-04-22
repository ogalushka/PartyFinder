using Autofac;
using System.Windows;

namespace WPFClient.Autofac
{
    public static class Extensions
    {
        public static ContainerBuilder RegisterViewWithBinding<TViewModel, TView>(this ContainerBuilder builder, FrameworkElement context) where TViewModel : notnull
        {
            var dataTemplate = new DataTemplate();
            dataTemplate.DataType = typeof(TViewModel);

            var loginViewFactory = new FrameworkElementFactory(typeof(TView));
            dataTemplate.VisualTree = loginViewFactory;

            context.Resources.Add(new DataTemplateKey(dataTemplate.DataType), dataTemplate);

            builder.RegisterType<TViewModel>();

            return builder;
        }
    }
}
