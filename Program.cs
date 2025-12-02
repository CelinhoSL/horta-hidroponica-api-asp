using DotNetEnv;
using System.Linq;
using Horta.Application.Service;
using Horta.Domain.Model;
using Horta_Api.Application.Interfaces;
using Horta_Api.Application.Service;
using Horta_Api.Application.Services;
using Horta_Api.Domain.Repositories;
using Horta_Api.Infrastructure.Middleware;
using Horta_Api.Infrastructure.Repositories;
using Horta_Api.Infrastructure.Security;
using Horta_Api.Infrastructure.Services;
using Horta_Api.Infrastructure.WebSockets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using MyGardenMonitoringWebSocketManager = Horta_Api.Infrastructure.WebSockets.GardenMonitoringWebSocketManager;
using MyGardenGetSensorValue = Horta_Api.Infrastructure.WebSockets.GardenGetSensorValue;
using MySensorUpdater = Horta_Api.Infrastructure.WebSockets.SensorUpdater;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
    builder.Logging.AddFilter("Horta_Api", LogLevel.Information);
    builder.Logging.AddFilter("Horta.Application", LogLevel.Information);
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Error);
}

// Add services to the container
builder.Services.AddControllers();

// ✅ Swagger - sempre adicionado (AddSwaggerGen)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});


if (builder.Environment.IsProduction())
{
    var connectionString = builder.Configuration[""]
                        ?? builder.Configuration[""];

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string do Azure PostgreSQL não encontrada.");
    }

    builder.Services.AddDbContext<ConnectionContext>(options => options.UseNpgsql(connectionString));
}
else
{
    builder.Services.AddDbContext<ConnectionContext>();
}

// Registros de serviços
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserVerificationCodeRepository, UserVerificationCodeRepository>();
builder.Services.AddScoped<IEmailVerificationService, EmailVerificationService>();
builder.Services.AddScoped<IEmailLogService, EmailLogService>();
builder.Services.AddScoped<IUserLogService, UserLogService>();
builder.Services.AddScoped<IUserLogRepository, UserLogRepository>();
builder.Services.AddScoped<IMainMcuConfigService, MainMcuConfigService>();
builder.Services.AddScoped<IMainMcuConfigRepository, MainMcuConfigRepository>();
builder.Services.AddScoped<IMainMcuRepository, MainMcuRepository>();
builder.Services.AddScoped<ILightSensorService, LightSensorService>();
builder.Services.AddScoped<IWaterLevelSensorService, WaterLevelSensorService>();
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<ITemperatureSensorService, TemperatureSensorService>();
builder.Services.AddScoped<IUpdateUserService, UpdateUserService>();
builder.Services.AddScoped<IUserResetPasswordCodeRepository, UserResetPasswordCodeRepository>();
builder.Services.AddScoped<IEmailResetPasswordCodeService, EmailResetPasswordCodeService>();

// WebSockets
builder.Services.AddSingleton<IGardenMonitoringAndControllingWebSocketManager, MyGardenMonitoringWebSocketManager>();
builder.Services.AddSingleton<IGardenGetSensorValue, MyGardenGetSensorValue>();
builder.Services.AddScoped<ISensorUpdater, MySensorUpdater>();

builder.Services.AddScoped<IMainMcuService, MainMcuService>();

// Password Hasher
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<ICameraRepository, CameraRepository>();
builder.Services.AddScoped<ICameraService, CameraService>();

// Email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddSingleton<ICameraStream, GardenCameraStream>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});





// JWT
var key = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new InvalidOperationException("JWT_SECRET não definida");
var keyBytes = Encoding.UTF8.GetBytes(key);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Horta API v1");
    
});


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Ocorreu um erro interno no servidor\"}");
        });
    });

    app.Use(async (context, next) =>
    {
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");
        await next();
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
app.UseMiddleware<WebSocketAuthMiddleware>();

// Map WebSockets
app.Map("/ws/status/{controllerId:int}", async (HttpContext context, IGardenMonitoringAndControllingWebSocketManager wsManager, int controllerId) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        await wsManager.HandleAsync(socket, controllerId);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Map("/ws/get-sensor-value/{controllerId:int}", async (HttpContext context, IGardenGetSensorValue wsManager, int controllerId) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        await wsManager.HandleAsync(socket, controllerId);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Map("/ws/update-sensor/{controllerId:int}", async (HttpContext context, ISensorUpdater wsManager, int controllerId) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        await wsManager.HandleAsync(socket, controllerId);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Map("/ws/camera/{camId}", async (HttpContext context, int camId) =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Esta rota aceita apenas conexões WebSocket");
        return;
    }

    var clientType = context.Request.Query["type"].ToString();

    if (string.IsNullOrEmpty(clientType) || (clientType != "sender" && clientType != "viewer"))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Parâmetro 'type' obrigatório. Use: ?type=sender ou ?type=viewer");
        return;
    }

    var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Usuário não autenticado");
        return;
    }

    var cameraService = context.RequestServices.GetRequiredService<ICameraStream>();
    var webSocket = await context.WebSockets.AcceptWebSocketAsync();

    await cameraService.HandleConnectionAsync(webSocket, userId, camId, clientType);
});

app.MapGet("/api/camera/{camId}/status", (int camId, ICameraStream cameraService) =>
{
    return Results.Ok(new
    {
        camId,
        hasSender = cameraService.HasActiveSender(camId),
        viewerCount = cameraService.GetViewerCount(camId),
        isStreaming = cameraService.HasActiveSender(camId)
    });
});

app.MapControllers();

if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<ConnectionContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Verificando migrations pendentes...");

            var pendingMigrations = db.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Aplicando {pendingMigrations.Count} migrations: {string.Join(", ", pendingMigrations)}");
                db.Database.Migrate();
                logger.LogInformation("Migrations aplicadas com sucesso!");
            }
            else
            {
                logger.LogInformation("Nenhuma migration pendente.");
            }
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Erro ao aplicar migrations");
            throw; 
        }
    }
}

app.Run();