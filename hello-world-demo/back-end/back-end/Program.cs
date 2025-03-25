using System.Text;
using back_end.Models;
using back_end.Repositories;
using back_end.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Authentication; // For PII logging if needed

// Enable detailed logging for development (remove in production)
// IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

// 1️⃣ Add Services to the Container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Auth Demo",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Please add the token in the format: Bearer <your-token>"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Database Context (Entity Framework Core - SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization();

// Dependency Injection for Repositories & Services
builder.Services.AddScoped<IHostService, HostService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();

// Enable CORS for frontend communication
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policyBuilder =>
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader());
});

builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication with explicit defaults and logging events.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        RequireSignedTokens = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
        // Explicitly allow only HS256 so the handler treats this as a plain JWS token.
        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
        TokenDecryptionKey = null
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                // Extract and trim the token; remove extra quotes if present.
                var token = authHeader.Substring("Bearer ".Length).Trim();
                if (token.StartsWith("\"") && token.EndsWith("\""))
                {
                    token = token.Trim('\"');
                }
                context.Token = token;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("Jwt");
            logger.LogError("Authentication failed: {Message}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { error = "Unauthorized", details = context.ErrorDescription });
            return context.Response.WriteAsync(result);
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("Jwt");
            logger.LogInformation("Authentication succeeded for user: {User}", context.Principal.Identity.Name);
            return Task.CompletedTask;
        }
    };
});

// Build the Application
var app = builder.Build();

// Enable Swagger (For API Documentation)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;  // Makes Swagger available at the root URL.
});

// Use CORS
app.UseCors("AllowAllOrigins");

app.UseRouting();

// Enable Authentication & Authorization
app.UseAuthentication();

// Custom Middleware: Force authentication to run on every request, even without [Authorize].
app.Use(async (context, next) =>
{
    // This call forces the authentication middleware to validate the token (if present)
    var result = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
    if (result.Succeeded && result.Principal != null)
    {
        // Set the HttpContext.User so that it contains the validated token claims.
        context.User = result.Principal;
    }
    await next();
});

app.UseAuthorization();

// Map Controllers (API Endpoints)
app.MapControllers();

// Run the application
app.Run();

//TODO: Add Roles
//TODO: Add Seed Data and Roles (Admin)
//TODO: Make sure JWT works
//TODO: Move validation to appropriate class (EventValidator)
