using Infrastructure;
using ChessAnalyzerApi.Hubs;
using System.Net.Http.Headers;
using ChessAnalyzerApi.Registrators;
using Microsoft.EntityFrameworkCore;
using ChessAnalyzerApi.Services.Analyze;
using ChessAnalyzerApi.Services.Lichess;
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
    builder.Services.AddScoped<ILichess, LichessService>();
    builder.Services.AddScoped<IAnalyzeService, AnalyzerService>();

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