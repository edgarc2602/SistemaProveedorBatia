using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Services;
using SistemaVentasBatia.Repositories;
using SistemaVentasBatia.Converters;
using SistemaVentasBatia.Middleware;
using SistemaVentasBatia.Controllers;


namespace SistemaVentasBatia
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
            services.AddHttpClient();


            // Configurar CORS para permitir todas las solicitudes
            services.AddCors(options => {
                options.AddPolicy("AllowAllOrigins",
                    builder => {
                        builder.AllowAnyOrigin()    // Permite cualquier origen (cambiar en producci�n)
                               .AllowAnyMethod()    // Permite cualquier m�todo (GET, POST, etc.)
                               .AllowAnyHeader();   // Permite cualquier cabecera
                    });
            });



            // Repository Context
            services.AddSingleton<DapperContext>();

            //// Configure Options
            //services.Configure<ProductoOption>(Configuration.GetSection("ProdOpt"));

            // Mapper
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapper());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Rest API
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter());
            });

            // SPA
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            // Services
            services.AddScoped<ICatalogosService, CatalogosService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IEntregaService, EntregaService>();
            services.AddScoped<IFacturaService, FacturaService>();
            services.AddScoped<ICuentaService, CuentaService>();

            // Repositories
            services.AddScoped<ICatalogosRepository, CatalogosRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IEntregaRepository, EntregaRepository>();
            services.AddScoped<IFacturaRepository, FacturaRepository>();
            services.AddScoped<ICuentaRepository, CuentaRepository>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            // Habilitar CORS
            app.UseCors("AllowAllOrigins");

            app.UseAuthorization();

            app.UseMiddleware<CustomErrorHandler>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
