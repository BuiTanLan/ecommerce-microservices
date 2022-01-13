using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Security.ApiKey;
using BuildingBlocks.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

//https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/README.md
public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddCustomSwagger(this WebApplicationBuilder builder,
        IConfiguration configuration, Assembly assembly)
    {
        builder.Services.AddCustomSwagger(configuration, assembly);

        return builder;
    }

    public static IServiceCollection AddCustomSwagger(this IServiceCollection services,
        IConfiguration configuration, Assembly assembly, bool useApiVersioning = false, bool tagByActionName = false)
    {
        if (useApiVersioning)
            services.AddVersionedApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

        // swagger docs for route to code style --> works in .net 6
        //https://dotnetthoughts.net/openapi-support-for-aspnetcore-minimal-webapi/
        //https://jaliyaudagedara.blogspot.com/2021/07/net-6-preview-6-introducing-openapi.html
        services.AddEndpointsApiExplorer();

        services.AddOptions<SwaggerOptions>().Bind(configuration.GetSection(nameof(SwaggerOptions)))
            .ValidateDataAnnotations();

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSwaggerGen(
            options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();
                var xmlFile = XmlCommentsFilePath(assembly);
                if (File.Exists(xmlFile)) options.IncludeXmlComments(xmlFile);

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                              Enter 'Bearer' [space] and then your token in the text input below.
                              \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityDefinition(ApiKeyConstants.HeaderName,
                    new OpenApiSecurityScheme
                    {
                        Description = "Api key needed to access the endpoints. X-Api-Key: My_API_Key",
                        In = ParameterLocation.Header,
                        Name = ApiKeyConstants.HeaderName,
                        Type = SecuritySchemeType.ApiKey
                    });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = ApiKeyConstants.HeaderName,
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme, Id = ApiKeyConstants.HeaderName
                            }
                        },
                        new string[] { }
                    }
                });

                if (useApiVersioning)
                {
                    // Grouping endopings by version and ApiExplorer group name.
                    options.DocInclusionPredicate((documentName, apiDescription) =>
                    {
                        var actionApiVersionModel = apiDescription.ActionDescriptor
                            .GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

                        if (actionApiVersionModel == null) return true;

                        var apiExplorerSettingsAttribute =
                            (ApiExplorerSettingsAttribute)apiDescription.ActionDescriptor
                                .EndpointMetadata.FirstOrDefault(x =>
                                    x.GetType().Equals(typeof(ApiExplorerSettingsAttribute)));

                        if (apiExplorerSettingsAttribute == null) return true;

                        if (actionApiVersionModel.DeclaredApiVersions.Any())
                            return actionApiVersionModel.DeclaredApiVersions.Any(v =>
                                $"v{v.MajorVersion}" == documentName);

                        return actionApiVersionModel.ImplementedApiVersions.Any(v =>
                            $"v{v.MajorVersion}" == documentName);
                    });

                    // Adding all the available versions.
                    var apiVersionDescriptionProvider = services.BuildServiceProvider()
                        .GetService<IApiVersionDescriptionProvider>();

                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        var openApiInfo = new OpenApiInfo
                        {
                            Title = $"{description.GroupName} API",
                            Version = description.ApiVersion.ToString(),
                            Description = $"{description.GroupName} API description."
                        };

                        if (description.IsDeprecated)
                            openApiInfo.Description += " This API version has been deprecated.";

                        options.SwaggerDoc(description.GroupName, openApiInfo);
                    }
                }

                if (tagByActionName)
                    //https://rimdev.io/swagger-grouping-with-controller-name-fallback-using-swashbuckle-aspnetcore/
                    options.TagActionsBy(api =>
                    {
                        if (api.GroupName != null) return new[] { api.GroupName };

                        if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                            return new[] { controllerActionDescriptor.ControllerName };

                        return new List<string>();
                    });

                // Adding swagger data annotation support.
                // options.EnableAnnotations();
            });


        return services;

        static string XmlCommentsFilePath(Assembly assembly)
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var fileName = assembly.GetName().Name + ".xml";
            return Path.Combine(basePath, fileName);
        }
    }
}
