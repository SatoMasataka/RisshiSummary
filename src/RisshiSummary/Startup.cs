using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;

namespace RisshiSummary
{
    /// <summary>
    /// コンフィグファイル値保持
    /// </summary>
    public static class ConfigValues
    {
        public static string MecabPath { get; set; }
    }

    public class Startup
    {
        //public static IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            var path = appEnv.ApplicationBasePath;
            Configuration= new ConfigurationBuilder().AddJsonFile($"{path}/appsettings.json").AddEnvironmentVariables().Build();
           


            

        }

        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(Microsoft.Framework.DependencyInjection.IServiceCollection services)
        {
            services.AddMvc();

        }
        public static IConfiguration Configuration { get; set; }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}
