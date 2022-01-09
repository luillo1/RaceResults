using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using RaceResults.Api.Authorization;
using RaceResults.Data.Core;
using RaceResults.Data.KeyVault;

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
            services.AddScoped<RequireOrganizationAuthorizationAttribute>();
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
