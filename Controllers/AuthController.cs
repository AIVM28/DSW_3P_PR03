
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DSW_3P_PR02.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DSW_3P_PR02.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // POST: api/auth/login
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto login)
    {
        // Obtener las credenciales desde User Secrets.
        var usuarioCorrecto = _configuration["Auth:Username"];
        var passwordCorrecta = _configuration["Auth:Password"];

        if (usuarioCorrecto == null || passwordCorrecta == null)
        {
            throw new InvalidOperationException(
                "No se configuraron las credenciales de autenticación."
            );
        }

        // Validar las credenciales.
        if (login.Usuario != usuarioCorrecto ||
            login.Password != passwordCorrecta)
        {
            return Unauthorized(new
            {
                mensaje = "Usuario o contraseña incorrectos."
            });
        }

        // Obtener la configuración JWT.
        var jwt = _configuration.GetSection("Jwt");

        var key = Convert.FromBase64String(
            jwt["Key"] ?? throw new InvalidOperationException(
                "No se configuró Jwt:Key."
            )
        );

        var issuer = jwt["Issuer"]
            ?? throw new InvalidOperationException(
                "No se configuró Jwt:Issuer."
            );

        var audience = jwt["Audience"]
            ?? throw new InvalidOperationException(
                "No se configuró Jwt:Audience."
            );

        var expirationMinutes = int.Parse(
            jwt["ExpirationMinutes"] ?? "30"
        );

        // Crear las credenciales de firma.
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256
        );

        // Definir la información del usuario.
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login.Usuario),
            new Claim(ClaimTypes.Role, "Administrador")
        };

        // Generar el token JWT.
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        // Convertir el token a una cadena.
        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        // Devolver el resultado.
        return Ok(new
        {
            access_token = tokenString,
            token_type = "Bearer",
            expires_in = expirationMinutes * 60
        });
    }
}
