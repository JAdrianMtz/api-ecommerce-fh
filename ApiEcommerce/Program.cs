using ApiEcommerce.Data;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Swagger;
using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Framework Services
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()!;
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddOutputCache(options => 
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60)
);

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

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.OperationFilter<AuthorizationFilter>();
});

// Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

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
app.UseCors();

app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapControllers();

app.Run();
