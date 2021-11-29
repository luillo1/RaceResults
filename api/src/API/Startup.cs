using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using RaceResults.Data.Core;

namespace RaceResults.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMicrosoftIdentityWebApiAuthentication(Configuration);
            services.AddControllers();
            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<IKeyVaultClient, KeyVaultClient>();
            services.AddSingleton<ICosmosDbClient>(services =>
                    {
                        string endpoint = this.Configuration["CosmosDb:Endpoint"];
                        string databaseName = this.Configuration["CosmosDb:DatabaseName"];
                        return new CosmosDbClient(endpoint, databaseName);
                    });
            services.AddSingleton<ICosmosDbContainerProvider>(services =>
                    {
                        ICosmosDbClient client = services.GetRequiredService<ICosmosDbClient>();
                        return new CosmosDbContainerProvider(client);
                    });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
