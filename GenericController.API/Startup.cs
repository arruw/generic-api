using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using GenericController.AspNetMvc;
using GenericController.API.Models;

namespace GenericController.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("Database"));
            });

            services.AddSwaggerGen(options => options
                .SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Generic API", Version = "v1" }));

            services.AddTransient(typeof(IApplicationRepository<>), typeof(ApplicationRepository<>));

            services.AddSingleton<IGenericObjectGraphTypeCache, GenericObjectGraphTypeCache>();

            services.AddMvc()
                .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    })
                .AddGenericControllers<ApplicationDbContext, IApplicationEntity>(typeof(GenericController<>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options => options
                .SwaggerEndpoint("/swagger/v1/swagger.json", "Generic API v1"));

            app.UseMvc();
        }
    }
}
