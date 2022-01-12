using System;
using Ardalis.GuardClauses;
using BuildingBlocks.Email.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Email;

public static class Extensions
{
    public static void AddSendGrid(this IServiceCollection services, IConfiguration configuration,
        Action<SendGridConfig> configure = null)
    {
        services.AddSingleton<IEmailSender, EmailSender>();

        var config = configuration.GetSection(nameof(SendGridConfig)).Get<SendGridConfig>();

        services.Configure<SendGridConfig>(configuration.GetSection(nameof(SendGridConfig)));
        if (configure is { })
            services.Configure(nameof(SendGridConfig), configure);
    }
}
