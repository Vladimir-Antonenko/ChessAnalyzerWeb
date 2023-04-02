using Infrastructure;
using ChessAnalyzerApi;
using ChessAnalyzerApi.Hubs;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using ChessAnalyzerApi.Services.Analyze;
using ChessAnalyzerApi.Services.Lichess;
using ChessAnalyzerApi.Services.Lichess.Mapping;

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

    // IPositionEvaluation
    builder.Services.AddHttpClient();
    //builder.Services.AddHttpClient<ILichess, LichessService>(cfg =>
    //{
    //    cfg.BaseAddress = new Uri("https://lichess.org/api/");
    //});


    builder.Services.AddHttpClient("LichessAPI",
        client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["LichessApiBaseUrl"]!);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", builder.Configuration["LichessToken"]);
            //lip_gleNJLbdji3tDh1zMvqk // токен
        });

    builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                                       .CreateClient("LichessAPI"));

    // пример для oauth2
    //options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    //{
    //    Type = SecuritySchemeType.OAuth2,
    //    Flows = new OpenApiOAuthFlows()
    //    {
    //        Implicit = new OpenApiOAuthFlow()
    //        {
    //            AuthorizationUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
    //            TokenUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),

    //            Scopes = new Dictionary<string, string>()
    //                    {
    //                        { "webshoppingagg", "Shopping Aggregator for Web Clients" }
    //                    }
    //        }
    //    }
    //});

    builder.Services.AddAutoMapper(config =>
            {
                config.AddProfile<EvaluationProfile>();
                config.AddProfile<PgnProfile>();
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