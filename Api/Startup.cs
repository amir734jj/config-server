using System;
using System.IO;
using System.Reflection;
using Dal;
using EfCoreRepository.Extensions;
using Logic;
using Logic.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using static Dal.Utilities.ConnectionStringUtility;

namespace Api
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpsRedirection(options => options.HttpsPort = 443);

            // If environment is localhost, then enable CORS policy, otherwise no cross-origin access
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()));
            
            services.AddMvc().AddNewtonsoftJson();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "config-server", Version = "v1",
                    Description = File.ReadAllText("description.txt"),
                    Contact =
                        new OpenApiContact
                        {
                            Name = "Amir",
                            Email = "amir@hesamian.com",
                            Url = new Uri("https://github.com/amir734jj")
                        }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            services.AddDbContext<EntityDbContext>(opt =>
            {
                var postgresConnectionString =
                    ConnectionStringUrlToPgResource(_configuration.GetValue<string>("DATABASE_URL")
                                                    ?? throw new Exception("DATABASE_URL is null"));
                opt.UseNpgsql(postgresConnectionString);
            });
            
            services.AddDbContext<EntityDbContext>(x => x.UseNpgsql(""));
            
            services.AddEfRepository<EntityDbContext>(x => x.Profiles(Assembly.Load("Dal")));

            services.AddScoped<IConfigLogic, ConfigLogic>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfigLogic configLogic)
        {
            configLogic.Cleanup().Wait();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseCors("CorsPolicy");
            
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1"));

            app.UseRouting()
                .UseEndpoints(endpoint => endpoint.MapDefaultControllerRoute());
        }
    }
}
