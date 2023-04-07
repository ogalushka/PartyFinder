using GamesCatalog.Http;
using GamesCatalog.Repository;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDataProtection().SetApplicationName("unique2").PersistKeysToFileSystem(new DirectoryInfo("../keys"));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

//builder.Services.AddDbContext<DbContext>(options => options.UseSqlServer(""));
builder.Services.AddSingleton<UsersRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<GamesHttpClient>(client => client.BaseAddress = new Uri("https://api.rawg.io/"));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
