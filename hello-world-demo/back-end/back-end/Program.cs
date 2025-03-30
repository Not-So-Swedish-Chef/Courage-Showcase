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
using Microsoft.AspNetCore.Authentication;
using back_end.DTOs;

// Enable detailed logging for development (remove in production)
IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

// 🔹 1️⃣ Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 🔹 2️⃣ Set Content Root
builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

// 🔹 3️⃣ Add Essential Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 🔹 4️⃣ Configure Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth Demo", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
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

// 🔹 5️⃣ Configure Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 6️⃣ Configure Identity (Must come **before** authentication)
builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// 🔹 8️⃣ Configure CORS (Before Authentication)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policyBuilder =>
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader());
});

// 🔹 9️⃣ Configure Authentication & JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => 
{ 
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Token authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"JWT Challenge error: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddAuthorization();
builder.Services.AddScoped<JwtService>();
// 🔹 7️⃣ Register Application Services
builder.Services.AddScoped<IHostService, HostService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();

// 🔹 1️⃣0️⃣ Build the Application
var app = builder.Build();

// 🔹 1️⃣1️⃣ Enable Swagger (For API Documentation)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;  // Makes Swagger available at the root URL.
});

app.UseCors("AllowAllOrigins"); // CORS comes before Authentication

app.UseRouting(); // Enables endpoint routing

app.UseAuthentication(); // Enables JWT authentication

app.UseAuthorization(); // Enables authorization policies

app.MapControllers(); // Maps API controllers

// 🔹 1️⃣3️⃣ Run the Application
app.Run();
