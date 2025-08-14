using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RickMortyApi.Database;
using RickMortyApi.Models;
using RickMortyApi.WebSockets;
using System.Net.WebSockets;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));


var connectionString = builder.Configuration.GetConnectionString("RickAndMortyDb");
builder.Services.AddDbContext<RickAndMortyDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key
    };
});
var wsHandler = new FavoritesWebSocketHandler();
builder.Services.AddSingleton<FavoritesWebSocketHandler>(wsHandler);
builder.Services.AddSingleton<JwtSettings>(jwtSettings);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddWebSockets(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "RickMortyApi", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your token."
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                    }
            });
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(builder =>
    builder
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
);

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        Console.WriteLine("Conexión WebSocket detectada en la ruta /ws/favorites");

        if (context.Request.Path == "/ws/favorites")
        {
            if (context.Request.Query.TryGetValue("email", out var email))
            {
                Console.WriteLine("Intentando establecer conexión WebSocket...");
                using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    wsHandler.TryAdd(email.ToString(), webSocket);

                    while (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.Connecting)
                    {
                        await Task.Delay(1000);
                    }
                }

                Console.WriteLine($"Conexión WebSocket establecida para el usuario: {email}");
            }
            else
            {
                Console.WriteLine("Solicitud WebSocket no válida.");
                context.Response.StatusCode = 400; // Bad Request
            }
        }
        else
        {
            await next();
        }
    }
    else
    {
        await next();
    }

});

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RickAndMortyDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        if (db.Database.CanConnect())
        {
            logger.LogInformation("Conexión a la base de datos exitosa.");
        }
        else
        {
            logger.LogError("No se pudo conectar a la base de datos.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error de conexión a la base de datos: {Message}", ex.Message);
    }
}

app.Run();
