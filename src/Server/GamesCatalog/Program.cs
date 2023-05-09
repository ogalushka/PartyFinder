using GamesCatalog.Http;
using GamesCatalog.Repository;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Common.MassTransit;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDataProtection().SetApplicationName("unique2").PersistKeysToFileSystem(new DirectoryInfo("../keys"));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddCookie(IdentityConstants.ApplicationScheme);

//builder.Services.AddDbContext<DbContext>(options => options.UseSqlServer(""));
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
