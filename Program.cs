
using Microsoft.EntityFrameworkCore;
using DSW_3P_PR02.Data;
using DSW_3P_PR02.Repositories;
using DSW_3P_PR02.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar OpenAPI.
builder.Services.AddOpenApi();

// Habilitar los controladores.
builder.Services.AddControllers();

// Obtener la cadena de conexión desde User Secrets.
var connectionString = builder.Configuration
    .GetConnectionString("SqlServerConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión SqlServerConnection."
    );

// Configurar SQL Server.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// Configurar la inyección de dependencias.
builder.Services.AddScoped<
    IPersonajeRepository,
    PersonajeSQLRepository
>();

builder.Services.AddScoped<
    IPersonajeService,
    PersonajeService
>();

// Configurar política de CORS.
builder.Services.AddCors(options =>
{
    options.AddPolicy("CharacterHubClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5500",
                "http://127.0.0.1:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configurar el entorno de desarrollo.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "CharacterHub API v1"
        );
    });
}

// Configurar las rutas de la aplicación.
app.UseHttpsRedirection();

app.UseCors("CharacterHubClient");

app.MapControllers();

app.Run();
