using GamesCatalog.Http;
using GamesCatalog.Repository;
using Common.MassTransit;
using Microsoft.AspNetCore.Identity;
using Common.Keys;
using GamesCatalog.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKeysStorage(builder.Configuration.GetConnectionString("Keys"));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddCookie(IdentityConstants.ApplicationScheme);
builder.Services.AddDbContext<PlayersDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Players")));

builder.Services.AddSingleton<UsersRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// TODO move api url to config
builder.Services.AddHttpClient<GamesHttpClient>(client => client.BaseAddress = new Uri("https://api.rawg.io/"));
builder.Services.AddMassTransitWithRabitMq();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
