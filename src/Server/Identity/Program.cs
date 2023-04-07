using Identity.Entity;
using Identity.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;
using UserService.Repository;


var builder = WebApplication.CreateBuilder(args);
//TODO save to keys to mongo
/*
var mongoClient = new MongoClient("mongodb://localhost:27017");
var database = mongoClient.GetDatabase("mydatabase");
var collection = database.GetCollection<BsonDocument>("dataprotectionkeys");
var repository = new XmlRepository(collection, "myapp");
 */
var mongoClient = new MongoClient();
var database = mongoClient.GetDatabase("keys");
var collection = database.GetCollection<BsonDocument>("keyss");
var repository = new MongoXmlRepository(collection, "dfsafd");
builder.Services.AddDataProtection().SetApplicationName("unique2");

builder.Services.AddDataProtection().SetApplicationName("unique2").PersistKeysToFileSystem(new DirectoryInfo("../keys"));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddControllers();

builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
//app.MapGet("/", () => "Hello world! from identity");
//app.MapGet("/protected", () => "secret").RequireAuthorization();
/*app.MapGet("/login", (HttpContext ctx) => {
    ctx.SignInAsync(
        new ClaimsPrincipal(new[] { 
            new ClaimsIdentity(new List<Claim>(), CookieAuthenticationDefaults.AuthenticationScheme) 
        } 
    ));
    return "ok";
});*/
app.Run();


/*




builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("manager", pb =>
    {
        pb.RequireAuthenticatedUser()
         .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
         .RequireClaim("role", "manager");
    });
});



*/
