using ApiEcommerce.Data;
using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// DB Config
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer("name=DefaultConnection")
);

// Automapper Config
builder.Services.AddAutoMapper(config =>
{
    config.AddMaps(typeof(Program));
    config.Internal().ForAllMaps((typeMap, expression) =>
    {
        expression.MaxDepth(32);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
