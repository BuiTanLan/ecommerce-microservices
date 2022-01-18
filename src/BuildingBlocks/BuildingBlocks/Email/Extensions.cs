using BuildingBlocks.Email.Configs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Email;

public static class Extensions
{
    public static WebApplicationBuilder AddEmailService(
        this WebApplicationBuilder builder,
        IConfiguration configuration,
        EmailProvider provider = EmailProvider.MimKit,
        Action<EmailConfig> configure = null)
    {
        AddEmailService(builder.Services, configuration, provider, configure);

        return builder;
    }

    public static IServiceCollection AddEmailService(
        this IServiceCollection services,
        IConfiguration configuration,
        EmailProvider provider = EmailProvider.MimKit,
        Action<EmailConfig> configurator = null)
    {
        if (provider == EmailProvider.SendGrid)
        {
            services.AddSingleton<IEmailSender, SendGridEmailSender>();
        }
        else
        {
            services.AddSingleton<IEmailSender, MimeKitEmailSender>();
        }

        var config = configuration.GetSection(nameof(EmailConfig)).Get<EmailConfig>();

        services.Configure<EmailConfig>(configuration.GetSection(nameof(EmailConfig)));
        if (configurator is { })
            services.Configure(nameof(EmailConfig), configurator);

        return services;
    }
}
