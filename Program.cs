
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using DSW_3P_PR02.Data;
using DSW_3P_PR02.Repositories;
using DSW_3P_PR02.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar OpenAPI y Swagger con autenticación JWT.
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        // Registrar el esquema Bearer.
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes =
            new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Introduce el token JWT."
                }
            };

        // Proteger las operaciones documentadas,
        // excepto el endpoint de inicio de sesión.
        foreach (var path in document.Paths)
    {
        if (path.Key.Equals(
            "/api/auth/login",
            StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        if (path.Value?.Operations == null)
        {
            continue;
        }

        foreach (var operation in path.Value.Operations.Values)
        {
            operation.Security ??= new List<OpenApiSecurityRequirement>();

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        }
    }

        return Task.CompletedTask;
    });
});

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

// Obtener configuración JWT desde User Secrets.
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Falta Jwt:Key.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Falta Jwt:Issuer.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Falta Jwt:Audience.");

var keyBytes = Convert.FromBase64String(jwtKey);

if (keyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "La clave JWT debe contener al menos 32 bytes."
    );
}

// Configurar autenticación JWT.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Habilitar autorización.
builder.Services.AddAuthorization();

// Configurar política de CORS.
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("DSW_3P_PR03", policy =>
//     {
//         policy.WithOrigins(
//                 "http://localhost:5500",
//                 "http://127.0.0.1:5500"
//             )
//             .AllowAnyHeader()
//             .AllowAnyMethod();
//     });
// });

var app = builder.Build();

// Configurar Swagger.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "DSW_3P_PR03 API v1"
        );
    });
}

// Configurar middleware.
app.UseHttpsRedirection();

app.UseCors("DSW_3P_PR03");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
