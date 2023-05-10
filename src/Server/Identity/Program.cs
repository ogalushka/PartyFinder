using Common.Keys;
using Common.MassTransit;
using Identity.Entity;
using Identity.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKeysStorage(builder.Configuration.GetConnectionString("Keys"));

builder.Services.AddDbContext<IdentityDbContext>(c =>
        c.UseSqlServer(builder.Configuration.GetConnectionString("Identity"))
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
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();


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
