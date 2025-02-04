using Microsoft.EntityFrameworkCore;
using Users.Microservice.Data.DbContexts;
using Users.Microservice.Extentions;
using Users.Microservice.Mappers;
using Users.Microservice.Models.Enums;
using Users.Microservice.Services.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<UserDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllPolicy", policy => policy.RequireRole(
        Enum.GetName(UserRole.Admin),
        Enum.GetName(UserRole.Mentor),
        Enum.GetName(UserRole.User)));

    options.AddPolicy("MentorPolicy", policy => policy.RequireRole(
        Enum.GetName(UserRole.Admin),
        Enum.GetName(UserRole.Mentor)));

    options.AddPolicy("UserPolicy", policy => policy.RequireRole(
        Enum.GetName(UserRole.Admin),
        Enum.GetName(UserRole.User)));

    options.AddPolicy("AdminPolicy", policy => policy.RequireRole(
        Enum.GetName(UserRole.Admin)));
});

builder.Services.AddControllers();
builder.Services.AddCustomServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Services.GetService<IHttpContextAccessor>() != null)
    HttpContextHelper.Accessor = app.Services.GetRequiredService<IHttpContextAccessor>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
