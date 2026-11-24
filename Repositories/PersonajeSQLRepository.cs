
using DSW_3P_PR02.Data;
using DSW_3P_PR02.Models;

namespace DSW_3P_PR02.Repositories;

public class PersonajeSQLRepository : IPersonajeRepository
{
    private readonly AppDbContext _context;

    public PersonajeSQLRepository(AppDbContext context)
    {
        _context = context;
    }

    // Obtener todos los personajes.
    public List<Personaje> ObtenerTodos()
    {
        return _context.Personajes.ToList();
    }

    // Obtener un personaje por su identificador.
    public Personaje? ObtenerPorId(int id)
    {
        return _context.Personajes.Find(id);
    }

    // Obtener personajes por videojuego.
    public List<Personaje> ObtenerPorVideojuego(string nombre)
    {
        return _context.Personajes
            .Where(p => p.Videojuego == nombre)
            .ToList();
    }

    // Obtener personajes con nivel igual o superior.
    public List<Personaje> ObtenerPorNivel(int nivel)
    {
        return _context.Personajes
            .Where(p => p.Nivel >= nivel)
            .ToList();
    }

    // Registrar un nuevo personaje.
    public Personaje Agregar(Personaje nuevoPersonaje)
    {
        nuevoPersonaje.Id = 0;

        _context.Personajes.Add(nuevoPersonaje);
        _context.SaveChanges();

        return nuevoPersonaje;
    }

    // Actualizar un personaje existente.
    public bool Actualizar(int id, Personaje personajeActualizado)
    {
        var actual = _context.Personajes.Find(id);

        if (actual == null)
            return false;

        actual.Nombre = personajeActualizado.Nombre;
        actual.Videojuego = personajeActualizado.Videojuego;
        actual.Clase = personajeActualizado.Clase;
        actual.Nivel = personajeActualizado.Nivel;
        actual.Poder = personajeActualizado.Poder;
        actual.Disponible = personajeActualizado.Disponible;
        actual.ImagenUrl = personajeActualizado.ImagenUrl;

        _context.SaveChanges();

        return true;
    }

    // Eliminar un personaje existente.
    public bool Eliminar(int id)
    {
        var personaje = _context.Personajes.Find(id);

        if (personaje == null)
            return false;

        _context.Personajes.Remove(personaje);
        _context.SaveChanges();

        return true;
    }
}
