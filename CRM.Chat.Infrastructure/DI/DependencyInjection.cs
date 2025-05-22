using System.Reflection;
using System.Security.Cryptography;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Publishers;
using CRM.Chat.Application.Common.Services.Auth;
using CRM.Chat.Application.Common.Services.Identity;
using CRM.Chat.Application.Common.Services.Outbox;
using CRM.Chat.Domain.Common.Events;
using CRM.Chat.Infrastructure.Contexts;
using CRM.Chat.Infrastructure.Publishers;
using CRM.Chat.Infrastructure.Services.Auth;
using CRM.Chat.Infrastructure.Services.IdentityService;
using CRM.Chat.Infrastructure.Services.Outbox;
using CRM.Identity.Domain.Common.Options.Auth;
using CRM.Identity.Infrastructure.Publishers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Chat.Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingeltonServices();
        services.AddScopedServices();
        services.AddDomainEventHandlers();
        services.ConfigureCors();
        services.AddAsymmetricAuthentication(configuration);
        services.AddOptions(configuration);
        //services.AddHostedService<OutboxProcessorWorker>();

        return services;
    }

    private static void ConfigureCors(this IServiceCollection services)
    {
        // TODO Restrict In Future Base On Origins Options
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }

    private static void AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!;
        services.AddSingleton(jwtOptions);
    }

    private static void AddSingeltonServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IJwtTokenService, JwtTokenService>();
        services.TryAddSingleton<IExternalEventPublisher, RabbitMQEventPublisher>();
    }

    public static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
    {
        // Register all handlers from assembly
        var assembly = Assembly.GetExecutingAssembly();

        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var handlerInterfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(handlerInterface, handlerType);
            }
        }

        return services;
    }

    private static void AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();
    }

    private static void AddAsymmetricAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!;

        byte[] publicKeyBytes = Convert.FromBase64String(jwtOptions.PublicKey);

        RSA rsaPublicKey = RSA.Create();
        rsaPublicKey.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        var issuerSigningKey = new RsaSecurityKey(rsaPublicKey);


        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = issuerSigningKey,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
    }
}