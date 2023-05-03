using Common.MassTransit;
using Identity.Entity;
using Identity.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);
//TODO save to keys database
/*
var mongoClient = new MongoClient();
var database = mongoClient.GetDatabase("keys");
var collection = database.GetCollection<BsonDocument>("keyss");
var repository = new MongoXmlRepository(collection, "dfsafd");
builder.Services.AddDataProtection().SetApplicationName("unique2");
*/
builder.Services.AddDataProtection().SetApplicationName("unique2").PersistKeysToFileSystem(new DirectoryInfo("../keys"));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddControllers();

builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddMassTransitWithRabitMq();

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
app.Run();
