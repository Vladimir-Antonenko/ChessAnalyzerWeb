using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace ChessAnalyzerApi.Configurations;

public static class LoggerConfig
{
    /// <summary>
    /// Добавляет serilog и настраивает сохранение в elastic. Сам ELK в докере можно поднять например так: https://github.com/deviantony/docker-elk
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostBuilder AddSerilogWithELK(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        return builder.Host.UseSerilog((hostContext, services, configuration) =>
        {
            configuration.MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .MinimumLevel.Override("System", LogEventLevel.Error)
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticSettings:Uri"]!))
            {
                IndexFormat = $"ChessAnalyzerWeb-{DateTime.UtcNow:yyyy-MM}",
                AutoRegisterTemplate = true,
                OverwriteTemplate = true,
                TemplateName = "logs",
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8, //.ESv7
                TypeName = null,
                BatchAction = ElasticOpType.Create,
                ModifyConnectionSettings = x => x.BasicAuthentication(
                                                                        builder.Configuration["ElasticSettings:Login"]!,
                                                                        builder.Configuration["ElasticSettings:Pass"]!
                                                                     ),
                FailureCallback = (ex) => Console.WriteLine("Unable to submit event " + ex.MessageTemplate)
            });
        });
    }
}