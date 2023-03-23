using ChessAnalyzerApi.Hubs;
using ChessAnalyzerApi.ExternalApi.Lichess.Mapping;
using ChessAnalyzerApi.ExternalApi.Lichess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSignalR();

builder.Services.AddHttpClient<LichessService>(cfg =>
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
