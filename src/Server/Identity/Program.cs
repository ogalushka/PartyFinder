using Common.MassTransit;
using Identity.Entity;
using Identity.Repository;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//TODO save to keys database

builder.Services.AddDbContext<IdentityRepository>(c =>
        c.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
    );
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o =>
{
    if (builder.Environment.IsDevelopment())
    {
        o.User.RequireUniqueEmail = false;
        o.Password.RequireDigit = false;
        o.Password.RequiredLength = 0;
        o.Password.RequireLowercase = false;
        o.Password.RequireUppercase = false;
        o.Password.RequireNonAlphanumeric = false;
    }
})
    .AddEntityFrameworkStores<IdentityRepository>()
    .AddDefaultTokenProviders();

builder.Services.AddDataProtection().SetApplicationName("unique2").PersistKeysToFileSystem(new DirectoryInfo("../keys"));

builder.Services.AddControllers();

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
