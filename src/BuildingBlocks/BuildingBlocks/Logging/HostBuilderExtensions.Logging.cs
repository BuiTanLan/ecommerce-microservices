using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Logging;
using Configuration;
using DependencyInjection;
using Hosting;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.SpectreConsole;

public static class HostBuilderExtensions
{
    private const string LoggerSectionName = "Logging";

    public static WebApplicationBuilder AddCustomSerilog(this WebApplicationBuilder builder,
        Action<LoggerConfiguration> extraConfigure = null)
    {
        AddCustomSerilog(builder.Host, extraConfigure);

        return builder;
    }


    public static IHostBuilder AddCustomSerilog(this IHostBuilder builder,
        Action<LoggerConfiguration> extraConfigure = null)
    {
        return builder.UseSerilog((context, serviceProvider, loggerConfiguration) =>
        {
            var httpContext = serviceProvider.GetService<IHttpContextAccessor>();
            loggerConfiguration
                .WriteTo.SpectreConsole(
                    "{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}",
                    minLevel: LogEventLevel.Information)
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(serviceProvider)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext();

            var loggerOptions = context.Configuration.GetSection(nameof(LoggerOptions)).Get<LoggerOptions>();
            if (loggerOptions is { })
                MapOptions(loggerOptions, loggerConfiguration, context);

            extraConfigure?.Invoke(loggerConfiguration);
        });
    }

    private static void MapOptions(LoggerOptions loggerOptions,
        LoggerConfiguration loggerConfiguration, HostBuilderContext hostBuilderContext)
    {
        var level = GetLogEventLevel(loggerOptions.Level);

        loggerConfiguration
            .MinimumLevel.Is(level)
            .Enrich.WithProperty("Environment", hostBuilderContext.HostingEnvironment.EnvironmentName);


        if (hostBuilderContext.HostingEnvironment.IsDevelopment())
        {
            loggerConfiguration.WriteTo.Console();
        }
        else
        {
            if (loggerOptions.UseElasticSearch)
                loggerConfiguration.WriteTo.Elasticsearch(loggerOptions.ElasticSearchLoggingOptions?.Url);
            if (loggerOptions.UseSeq)
                loggerConfiguration.WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ??
                                                loggerOptions.SeqOptions.Url);
            loggerConfiguration.WriteTo.Console();
        }

        foreach (var (key, value) in loggerOptions.Tags ?? new Dictionary<string, object>())
            loggerConfiguration.Enrich.WithProperty(key, value);

        foreach (var (key, value) in loggerOptions.MinimumLevelOverrides ?? new Dictionary<string, string>())
        {
            var logLevel = GetLogEventLevel(value);
            loggerConfiguration.MinimumLevel.Override(key, logLevel);
        }

        loggerOptions.ExcludePaths?.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty<string>("RequestPath", n => n.EndsWith(p))));

        loggerOptions.ExcludeProperties?.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty(p)));
    }

    private static LogEventLevel GetLogEventLevel(string level)
    {
        return Enum.TryParse<LogEventLevel>(level, true, out var logLevel)
            ? logLevel
            : LogEventLevel.Information;
    }
}
