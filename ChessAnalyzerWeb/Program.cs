using Infrastructure;
using Domain.GameAggregate;
using ChessAnalyzerApi.Hubs;
using System.Net.Http.Headers;
using ChessAnalyzerApi.Services;
using Microsoft.EntityFrameworkCore;
using ChessAnalyzerApi.Services.Analyze;
using ChessAnalyzerApi.Services.Lichess;
using ChessAnalyzerApi.Services.ChessDB;
using ChessAnalyzerApi.Services.Lichess.Mapping;
using ChessAnalyzerApi.Services.ChessDB.Mapping;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddDbContext<BaseContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
               .EnableSensitiveDataLogging()); 

    builder.Services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen()
        .RegisterRepositories()
        .AddSignalR();

    builder.Services.AddEvaluationServices();
    // builder.Services.AddScoped<ILichess, LichessService>(); // было
    builder.Services.AddTransient<LichessService>();
   // builder.Services.AddTransient<ChessComService>(); // пока не реализован
    builder.Services.AddScoped<Func<ChessPlatform, IPgn>> (serviceProvider => key =>
    {
        switch (key)
        {
            case ChessPlatform.Lichess:
                return serviceProvider.GetRequiredService<LichessService>(); // GetRequiredService with exception if not registrated!
            //case ChessPlatform.ChessCom:
            //    return serviceProvider.GetRequiredService<ChessComService>();
            default:
                throw new KeyNotFoundException();
        }
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

    builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ChessDB"));
    builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("LichessAPI"));

    builder.Services.AddAutoMapper(config =>
            {
                config.AddProfile<EvaluationProfile>();
                config.AddProfile<PgnProfile>();
                config.AddProfile<QueryPvProfile>();
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
    app.UseCors(cfg => cfg.AllowAnyOrigin()); // пока так дл€ тестировани€

    app.UseHttpsRedirection();

    app.MapControllers();
    app.MapHub<NotificationHub>("/notifications");

    app.Run();
}
catch(Exception ex)
{
   Console.WriteLine(ex.ToString());
}