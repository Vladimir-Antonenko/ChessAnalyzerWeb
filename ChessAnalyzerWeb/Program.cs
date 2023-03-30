using Infrastructure;
using ChessAnalyzerApi;
using ChessAnalyzerApi.Hubs;
using Microsoft.EntityFrameworkCore;
using ChessAnalyzerApi.Services.Analyze;
using ChessAnalyzerApi.Services.Lichess;
using ChessAnalyzerApi.Services.Lichess.Mapping;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

    builder.Services.AddDbContext<BaseContext>(options =>
     options.UseSqlite("Data Source=BaseAnalyzeGames.db")); // доработать чтобы вынести строку подключения в файл

   // options.UseSqlite("ConnectionStrings:DefaultConnection"));
    //  UseSqlite($"Data Source=BaseAnalyzeGames.db"

    builder.Services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen()
        .RegisterRepositories()
        .AddSignalR();

    builder.Services.AddEvaluationServices();
    builder.Services.AddScoped<ILichess, LichessService>();
    builder.Services.AddScoped<IAnalyzeService, AnalyzerService>();

    builder.Services.AddHttpClient<ILichess, LichessService>(cfg =>
    {
        cfg.BaseAddress = new Uri("https://lichess.org/api/");
    });

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

    app.UseHttpsRedirection();

    app.MapControllers();
    app.MapHub<NotificationHub>("/notifications");

    app.Run();
}
catch(Exception ex)
{
   Console.WriteLine(ex.ToString());
}