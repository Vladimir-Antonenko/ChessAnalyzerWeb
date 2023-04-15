using Infrastructure;
using Domain.GameAggregate;
using ChessAnalyzerApi.Hubs;
using System.Net.Http.Headers;
using ChessAnalyzerApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ChessAnalyzerApi.Services.Analyze;
using ChessAnalyzerApi.Services.Lichess;
using ChessAnalyzerApi.Services.ChessDB;
using ChessAnalyzerApi.Services.ChessCom;
using ChessAnalyzerApi.Services.Lichess.Mapping;
using ChessAnalyzerApi.Services.ChessDB.Mapping;
using ChessAnalyzerApi.Services.ChessCom.Mapping;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers()
        .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); // для преобразования параметров с клиента в enum'ку
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddDbContext<BaseContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
               .EnableSensitiveDataLogging()); 

    builder.Services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen()
        .RegisterRepositories()
        .AddSignalR().AddJsonProtocol(options =>    // чтобы попасть в метод хаба с клиента с enum'кой
        {
            options.PayloadSerializerOptions.Converters
                   .Add(new JsonStringEnumConverter());
        });

    builder.Services.AddEvaluationServices();

    // For strategy!
    builder.Services.AddTransient<LichessService>();
    builder.Services.AddTransient<ChessComService>();
    builder.Services.AddScoped<Func<ChessPlatform, IPgn>> (serviceProvider => key =>
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
        });

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

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.UseCors(cfg => cfg.AllowAnyOrigin()); // пока так для тестирования

    app.UseHttpsRedirection();

    app.MapControllers();
    app.MapHub<NotificationHub>("/notifications");

    app.Run();
}
catch(Exception ex)
{
   Console.WriteLine(ex.ToString());
}