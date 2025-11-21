using CiP_04_RockPaperArena.Application.Services;
using CiP_04_RockPaperArena.Domain.Interfaces;
using CiP_04_RockPaperArena.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins(
                "http://localhost:3000",  // React dev server
                "https://localhost:3001"  // In case React runs on HTTPS
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddSingleton<ITournamentRepository, TournamentRepository>();
builder.Services.AddSingleton<IParticipantRepository, ParticipantRepository>();
builder.Services.AddSingleton<IPairingStrategy, RoundRobinPairingStrategy>();
builder.Services.AddScoped<IGameService, GameService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowReactApp");


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
