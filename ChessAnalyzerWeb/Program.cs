using Infrastructure;
using Domain.GameAggregate;
using ChessAnalyzerApi.Hubs;
using System.Net.Http.Headers;
using ChessAnalyzerApi.Services;
using ChessAnalyzerApi.Middlewares;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ChessAnalyzerApi.Configurations;
using ChessAnalyzerApi.Services.Analyze;
using ChessAnalyzerApi.Services.Lichess;
using ChessAnalyzerApi.Services.ChessDB;
using ChessAnalyzerApi.Services.ChessCom;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ChessAnalyzerApi.Services.Lichess.Mapping;
using ChessAnalyzerApi.Services.ChessDB.Mapping;
using ChessAnalyzerApi.Services.ChessCom.Mapping;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddSerilogWithELK();

    builder.Services.AddControllers()
        .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); // for convert parameters from client to enum

    builder.Services.AddDbContext<BaseContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"),
                            q => q.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)) //Now, rather than making a single query with all the inner joins for adding navigation properties, it will split the query into one or more parts so that it doesn't impact the performance.
               .EnableSensitiveDataLogging()
               .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning)));

    builder.Services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen()
        .RegisterRepositories()
        .AddSignalR()
        .AddJsonProtocol(options =>    // call hub method from client with enum
        {
            options.PayloadSerializerOptions.Converters
                   .Add(new JsonStringEnumConverter());
        });

    builder.Services.AddEvaluationServices();

    // For strategy!
    builder.Services.AddTransient<LichessService>();
    builder.Services.AddTransient<ChessComService>();
    builder.Services.AddScoped<Func<ChessPlatform, IPgn>>(serviceProvider => key =>
    {
        return key switch
        {
            ChessPlatform.Lichess => serviceProvider.GetRequiredService<LichessService>(),// GetRequiredService with exception if not registrated!
            ChessPlatform.ChessCom => serviceProvider.GetRequiredService<ChessComService>(),
            _ => throw new KeyNotFoundException(),
        };
    });

    builder.Services.AddScoped<IAnalyzeService, AnalyzerService>();
    builder.Services.AddScoped<IChessDBService, ChessDBService>();

    builder.Services.AddHttpClient("ChessDB", client => client.BaseAddress = new Uri(builder.Configuration["ChessDBApiBaseUrl"]!));
    builder.Services.AddHttpClient("LichessAPI",
        client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["LichessApiBaseUrl"]!);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", builder.Configuration["LichessToken"]);
        });
    builder.Services.AddHttpClient("ChessComAPI",
        client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["ChessComApiBaseUrl"]!);
        }); //.AddHttpMessageHandler(((s) => s.GetService<MyCustomDelegatingHandler>()); // надо будет прикрутить

    builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ChessDB"));
    builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("LichessAPI"));
    builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ChessComAPI"));

    builder.Services.AddAutoMapper(config =>
    {
        config.AddProfile<LichessEvaluationProfile>();
        config.AddProfile<LichessPgnProfile>();
        config.AddProfile<QueryPvProfile>();
        config.AddProfile<ChessComPgnProfile>();
    });

    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<IMemoryCacheService, MistakesCacheService>();
    builder.Services.AddScoped<LoggingMiddleware>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseMiddleware<LoggingMiddleware>();
    }

    //app.UseMiddleware<LoggingMiddleware>();
    app.UseDefaultFiles();
    app.UseStaticFiles(new StaticFileOptions()
    {
        OnPrepareResponse = cfg =>
        {
            cfg.Context.Response.Headers.Add("Cache-Control", "public,max-age=300"); // 300 sec or 5 min
        }
    });
    app.UseCors(cfg => cfg.AllowAnyOrigin()); // пока так для тестирования
    app.UseHttpsRedirection();
    app.MapControllers();
    app.MapHub<NotificationHub>("/notifications");

    app.Run();
}
catch (Exception ex)
{
    //  logger.Fatal(ex);
    throw;
}
//finally
//{
//    NLog.LogManager.Shutdown();
//}