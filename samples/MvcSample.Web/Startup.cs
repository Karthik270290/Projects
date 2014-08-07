using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using MvcSample.Web.Filters;
using MvcSample.Web.Services;

#if NET45 
using Ninject;
using Microsoft.Framework.DependencyInjection.Ninject;
using Microsoft.Framework.OptionsModel;
#endif

namespace MvcSample.Web
{
    public class Startup
    {
        public void Configure(IBuilder app)
        {
            app.UseFileServer();
#if NET45
            var configuration = new Configuration()
                                    .AddJsonFile(@"App_Data\config.json")
                                    .AddEnvironmentVariables();

            string diSystem;

            if (configuration.TryGet("DependencyInjection", out diSystem) && 
                diSystem.Equals("AutoFac", StringComparison.OrdinalIgnoreCase))
            {
                var services = new ServiceCollection();

                services.AddMvc();
                services.AddSingleton<PassThroughAttribute>();
                services.AddSingleton<UserNameService>();
                services.AddTransient<ITestService, TestService>();                
                services.Add(OptionsServices.GetDefaultServices());

                // Create the Ninject container
                var kernel = new StandardKernel();

                // Create the container and use the default application services as a fallback 
                NinjectRegistration.Populate(
                    kernel,
                    services,
                    fallbackServiceProvider: app.ApplicationServices);

                app.UseServices(kernel.Get<IServiceProvider>());
            }
            else
#endif
            {
                app.UseServices(services =>
                {
                    services.AddMvc();
                    services.AddSingleton<PassThroughAttribute>();
                    services.AddSingleton<UserNameService>();
                    services.AddTransient<ITestService, TestService>();
                });
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action}");

                routes.MapRoute(
                    "controllerActionRoute",
                    "{controller}/{action}",
                    new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    "controllerRoute",
                    "{controller}",
                    new { controller = "Home" });
            });
        }
    }
}
