using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutofacSerilogIntegration;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RealTimeCharts_Server.Controllers;
using RealTimeCharts_Server.HubConfig;
using RealTimeCharts_Server.Models;
using Serilog;

namespace RealTimeCharts_Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public ContainerBuilder ApplicationBuilder { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => 
            { 
                options.AddPolicy("CorsPolicy", builder => builder.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()); 
            });

            services.AddAutofac();
            services.AddDbContext<EmployeeContext>(opts => opts.UseSqlServer(Configuration["ConnectionString:EmployeeDB"]));
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddSignalR();
            services.AddControllers();

            services.AddSwaggerGen();

            var mvcBuilder = services.AddMvc(options => options.EnableEndpointRouting = false);
            mvcBuilder.AddApplicationPart(typeof(ChartController).Assembly).AddControllersAsServices();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // do all the autofac registration here
            IOC.ServiceRegistration.Register(builder);
            IOC.ServiceRegistration.RegisterBackgroundServices(builder);
            builder.RegisterLogger();
            this.ApplicationBuilder = builder;
        }

        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // This will make the HTTP requests log as rich logs instead of plain text.
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "My Test1 Api v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChartHub>("/chart");
            });
        }
    }
}
