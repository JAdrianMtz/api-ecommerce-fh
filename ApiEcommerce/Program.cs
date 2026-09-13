using ApiEcommerce.Data;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Framework Services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer("name=DefaultConnection")
);

// External libraries
builder.Services.AddAutoMapper(config =>
{
    config.AddMaps(typeof(Program));
    config.Internal().ForAllMaps((typeMap, expression) =>
    {
        expression.MaxDepth(32);
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Api Ecommerce",
        Description = "Web api para trabajar con el Ecommerce"
    });
});

// Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// API Documentation
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Security
app.UseHttpsRedirection();

app.UseAuthorization();

// Endpoints
app.MapControllers();

app.Run();
