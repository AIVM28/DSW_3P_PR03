
using Microsoft.AspNetCore.Mvc;
using DSW_3P_PR02.Models;
using DSW_3P_PR02.Services;

namespace DSW_3P_PR02.Controllers;

[ApiController]
[Route("api/personajes")]
public class PersonajesController : ControllerBase
{
    private readonly IPersonajeService _service;

    public PersonajesController(IPersonajeService service)
    {
        _service = service;
    }

    // GET: api/personajes
    // Obtener todos los personajes.
    [HttpGet]
    [ProducesResponseType(typeof(List<Personaje>), StatusCodes.Status200OK)]
    public IActionResult ObtenerTodos()
    {
        return Ok(_service.ObtenerTodos());
    }

    // GET: api/personajes/1
    // Obtener un personaje por su identificador.
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Personaje), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult ObtenerPorId(int id)
    {
        var personaje = _service.ObtenerPorId(id);

        if (personaje == null)
        {
            return NotFound(new
            {
                mensaje = "El personaje no existe."
            });
        }

        return Ok(personaje);
    }

    // GET: api/personajes/videojuego/Minecraft
    // Obtener personajes por videojuego.
    [HttpGet("videojuego/{nombre}")]
    [ProducesResponseType(typeof(List<Personaje>), StatusCodes.Status200OK)]
    public IActionResult ObtenerPorVideojuego(string nombre)
    {
        try
        {
            return Ok(_service.ObtenerPorVideojuego(nombre));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensaje = ex.Message
            });
        }
    }

    // GET: api/personajes/nivel/50
    // Obtener personajes con nivel igual o superior.
    [HttpGet("nivel/{nivel:int}")]
    [ProducesResponseType(typeof(List<Personaje>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ObtenerPorNivel(int nivel)
    {
        try
        {
            return Ok(_service.ObtenerPorNivel(nivel));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensaje = ex.Message
            });
        }
    }

    // POST: api/personajes
    // Registrar un nuevo personaje.
    [HttpPost]
    [ProducesResponseType(typeof(Personaje), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Agregar([FromBody] Personaje nuevoPersonaje)
    {
        try
        {
            var creado = _service.Agregar(nuevoPersonaje);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = creado.Id },
                creado
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensaje = ex.Message
            });
        }
    }

    // PUT: api/personajes/1
    // Actualizar un personaje existente.
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Actualizar(
        int id,
        [FromBody] Personaje personajeActualizado)
    {
        try
        {
            if (!_service.Actualizar(id, personajeActualizado))
            {
                return NotFound(new
                {
                    mensaje = "El personaje no existe."
                });
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensaje = ex.Message
            });
        }
    }

    // DELETE: api/personajes/1
    // Eliminar un personaje existente.
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Eliminar(int id)
    {
        if (!_service.Eliminar(id))
        {
            return NotFound(new
            {
                mensaje = "El personaje no existe."
            });
        }

        return NoContent();
    }
}
