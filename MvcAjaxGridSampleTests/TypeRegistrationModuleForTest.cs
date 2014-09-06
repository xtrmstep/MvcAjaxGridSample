using Autofac;
using MvcAjaxGridSample.Types.DataModel;
using MvcAjaxGridSample.Types.DataStorage;
using MvcAjaxGridSampleTests.Mocks;

namespace MvcAjaxGridSampleTests
{
    public class TypeRegistrationModuleForTest : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TestRepository>().As<IRepository<Book>>().InstancePerLifetimeScope();
        }
    }
}