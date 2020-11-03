using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Velvetech.Identity.API.Data;
using Velvetech.Identity.API.Identity;
using Velvetech.Identity.API.Models;

namespace Velvetech.Identity.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Ctor 
        /// </summary>
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            _configuration = configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("serilog.json", true, true)
                .AddJsonFile($"serilog.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .CreateLogger();
        }

        /// <summary>
        /// Конфигурация сервисов
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TokenOptions>(_configuration.GetSection("TokenOptions"));

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRefreshTokenService, CacheRefreshTokenService>();

            services.AddDistributedMemoryCache();

            services
                .RegisterLoggingService()
                .RegisterSwaggerService()
                .RegisterDbContext(_configuration)
                .RegisterAutoMapper()
                .RegisterAuth(_configuration)
                .AddControllers()
                .AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreNullValues = true; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();

                // Подключение middleware Swagger JSON endpoint.
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API V1"); });
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }

    /// <summary>
    /// Расширение для сервисов
    /// <remarks>Настройка сервисов</remarks>> 
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Регистрация DB context 
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <param name="configuration">Конфигурация</param>
        public static IServiceCollection RegisterDbContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            var conString = configuration.GetConnectionString("Context");
            services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(conString));

            return services;
        }

        /// <summary>
        /// Регистрировать сервис для логирования
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        public static IServiceCollection RegisterLoggingService(this IServiceCollection services)
        {
            // Serilog for DI
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            return services;
        }

        /// <summary>
        /// Регистрировать аутентификацию 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterAuth(this IServiceCollection services,
            IConfiguration configuration)
        {
            var tokenOptions = new TokenOptions();
            configuration.GetSection("TokenOptions").Bind(tokenOptions);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(op =>
                {
                    op.RequireHttpsMetadata = false;

                    op.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Валидация издателя
                        ValidateIssuer = true,
                        // строка, представляющая издателя
                        ValidIssuer = tokenOptions.Issuer,

                        // Валидация потребителя токена
                        ValidateAudience = true,
                        // установка потребителя токена
                        ValidAudience = tokenOptions.Audience,

                        // Валидация время жизни токена
                        ValidateLifetime = true,

                        // Установка ключа безопасности
                        IssuerSigningKey = tokenOptions.GetSymmetricSecurityKey(),
                        // Валидация ключа безопасности
                        ValidateIssuerSigningKey = true
                    };
                });

            return services;
        }

        /// <summary>
        /// Регистрировать AutoMapper 
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            return services;
        }

        /// <summary>
        /// Регистрировать сервис для Swagger
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        public static IServiceCollection RegisterSwaggerService(this IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Velvetech.Identity.API",
                    Version = "v1",
                    Description = "Velvetech.Identity API"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Введите в поле Bearer с JWT токеном как показано выше (без скобок)"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}