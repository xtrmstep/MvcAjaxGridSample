using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using SampleAjaxFormInMVC;
using Module = Autofac.Module;

namespace MvcAjaxGridSample
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = RegisterTypes(new TypeRegistrationModule());
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            BundleConfig.RegisterBundle(BundleTable.Bundles);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        public static IContainer RegisterTypes(Module module)
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterModule(module);
            return builder.Build();
        }
    }
}