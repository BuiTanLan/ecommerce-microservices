using BuildingBlocks.Email.Options;
using BuildingBlocks.Web.Extensions;
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
        Action<EmailOptions>? configure = null)
    {
        AddEmailService(builder.Services, configuration, provider, configure);

        return builder;
    }

    public static IServiceCollection AddEmailService(
        this IServiceCollection services,
        IConfiguration configuration,
        EmailProvider provider = EmailProvider.MimKit,
        Action<EmailOptions>? configurator = null)
    {
        if (provider == EmailProvider.SendGrid)
        {
            services.AddSingleton<IEmailSender, SendGridEmailSender>();
        }
        else
        {
            services.AddSingleton<IEmailSender, MimeKitEmailSender>();
        }

        var config = configuration.GetOptions<EmailOptions>(nameof(EmailOptions));

        services.Configure<EmailOptions>(configuration.GetSection(nameof(EmailOptions)));
        if (configurator is { })
            services.Configure(nameof(EmailOptions), configurator);

        return services;
    }
}
