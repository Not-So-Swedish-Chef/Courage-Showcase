using AutoMapper;
using back_end.DTOs;
using back_end.Models;
using back_end.Repositories;
using back_end.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policyBuilder =>
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader());
});

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

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
}); 
builder.Services.AddAuthorization();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IHostService, HostService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    await SeedRolesAsync(roleManager); 
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAllOrigins");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers(); 
app.Run();

//Seed roles
static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
{
    string[] roles = { "Member", "Host", "Admin" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
            if (!result.Succeeded)
            {
                var msg = string.Join("; ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new InvalidOperationException($"Failed creating role '{role}': {msg}");
            }
        }
    }
}