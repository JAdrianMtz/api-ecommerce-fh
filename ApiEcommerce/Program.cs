using ApiEcommerce.Configurations;
using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Swagger;
using Asp.Versioning;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Framework Services
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

// Configuration
builder.Services.AddOptions<JwtSettings>()
    .BindConfiguration(JwtSettings.Seccion)
    .ValidateDataAnnotations()
    .ValidateOnStart();

var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.Seccion)
    .Get<JwtSettings>()!;

// Authentication
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// CORS
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>()!;

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Output Cache
builder.Services.AddOutputCache(options =>
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60)
);

// Versioning
var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    // options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version")); //?api-version
});

apiVersioningBuilder.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // v1,v2,v3...
    options.SubstituteApiVersionInUrl = true; // api/v{version}/products
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("name=DefaultConnection")
        .UseSeeding((context, _) =>
        {
            var appContext = (ApplicationDbContext)context;
            DataSeeder.SeedData(appContext);
        })
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
        Title = "Api Ecommerce V1",
        Description = "Web api para trabajar con el Ecommerce"
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "Api Ecommerce V2",
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
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Ecommerce V1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Api Ecommerce V2");
    });
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
