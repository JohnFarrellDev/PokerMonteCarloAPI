using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PokerMonteCarloAPI.Services;

namespace PokerMonteCarloAPI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddFluentValidation(fv =>
                    fv.RegisterValidatorsFromAssemblyContaining<Startup>(lifetime: ServiceLifetime.Singleton));

            services.AddScoped<IMonte, Monte>();
            services.AddSingleton<IRandomService, RandomService>();

            services.AddSwaggerDocument(configure => configure.Title = "Poker Monte Carlo API");
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHsts();
            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
        
        
        // TODO Implementation of an API level test
        // TODO API healthcheck endpoint
        // TODO - implement actual monte carlo solution
        //      TODO - Create empty method on Player class to calculate best hand
        //      TODO - Write tests for all possible hand types + high card combinations
        //      TODO - Start implementing logic to make tests pass
        // TODO - utilise stack for removal of last element
        // TODO - consider threads when running monte carlo loop
    }
}