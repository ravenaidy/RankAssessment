using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RES.ATM.API.ExceptionHandling.Exceptions;
using RES.ATM.API.Infrastructure.DBConnection;
using RES.ATM.API.Infrastructure.DBConnection.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Dapper;
using RES.ATM.API.Infrastructure.Repositories.Dapper.TypeHandlers;
using System.Reflection;
using RES.ATM.API.Domain.ATM.Handlers;
using RES.ATM.API.Domain.ATM.Events;

namespace RES.ATM.API
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
            services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory<SqliteConnection>(Configuration.GetConnectionString("ATM")));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RES.ATM.API", Version = "v1" });
            });            
            services.AddATMServices(Configuration);
            services.AddMediatR(typeof(WithdrawalCompletedEvent));

            // Add mapper to Guid and remove the out of the box one
            SqlMapper.AddTypeHandler(new SqlLiteGuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RES.ATM.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseCors(x =>
                x.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseMiddleware<ATMExceptionHandlerMiddleWare>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
