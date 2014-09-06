using Autofac;
using Autofac.Integration.Mvc;
using MvcAjaxGridSample.Types.DataModel;
using MvcAjaxGridSample.Types.DataStorage;
using MvcAjaxGridSample.Types.DataStorage.Implementation;

namespace MvcAjaxGridSample
{
    public class TypeRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Repository<Book>>().As<IRepository<Book>>().InstancePerHttpRequest();
        }
    }
}