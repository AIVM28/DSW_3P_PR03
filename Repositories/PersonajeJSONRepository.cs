
using System.Text.Json;
using DSW_3P_PR02.Models;

namespace DSW_3P_PR02.Repositories;

public class PersonajeJsonRepository : IPersonajeRepository
{
    private readonly string _ruta;

    private readonly JsonSerializerOptions _opciones = new()
    {
        WriteIndented = true
    };

    public PersonajeJsonRepository(IWebHostEnvironment environment)
    {
        _ruta = Path.Combine(
            environment.ContentRootPath,
            "Data",
            "personajes.json"
        );

        Directory.CreateDirectory(
            Path.GetDirectoryName(_ruta)!
        );

        if (!File.Exists(_ruta))
        {
            File.WriteAllText(_ruta, "[]");
        }
    }

    // Leer los personajes almacenados en el archivo JSON.
    private List<Personaje> Leer()
    {
        string json = File.ReadAllText(_ruta);

        return JsonSerializer.Deserialize<List<Personaje>>(json)
               ?? new List<Personaje>();
    }

    // Guardar los personajes en el archivo JSON.
    private void Guardar(List<Personaje> datos)
    {
        string json = JsonSerializer.Serialize(datos, _opciones);

        File.WriteAllText(_ruta, json);
    }

    // Obtener todos los personajes.
    public List<Personaje> ObtenerTodos()
    {
        return Leer();
    }

    // Buscar un personaje por su identificador.
    public Personaje? ObtenerPorId(int id)
    {
        return Leer().FirstOrDefault(p => p.Id == id);
    }

    // Buscar personajes por videojuego.
    public List<Personaje> ObtenerPorVideojuego(string nombre)
    {
        return Leer()
            .Where(p => p.Videojuego.Equals(
                nombre,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // Obtener personajes con nivel igual o superior.
    public List<Personaje> ObtenerPorNivel(int nivel)
    {
        return Leer()
            .Where(p => p.Nivel >= nivel)
            .ToList();
    }

    // Registrar un nuevo personaje.
    public Personaje Agregar(Personaje nuevoPersonaje)
    {
        var datos = Leer();

        nuevoPersonaje.Id = datos.Count == 0
            ? 1
            : datos.Max(p => p.Id) + 1;

        datos.Add(nuevoPersonaje);

        Guardar(datos);

        return nuevoPersonaje;
    }

    // Actualizar un personaje existente.
    public bool Actualizar(int id, Personaje personajeActualizado)
    {
        var datos = Leer();

        var existente = datos.FirstOrDefault(p => p.Id == id);

        if (existente == null)
        {
            return false;
        }

        existente.Nombre = personajeActualizado.Nombre;
        existente.Videojuego = personajeActualizado.Videojuego;
        existente.Clase = personajeActualizado.Clase;
        existente.Nivel = personajeActualizado.Nivel;
        existente.Poder = personajeActualizado.Poder;
        existente.Disponible = personajeActualizado.Disponible;
        existente.ImagenUrl = personajeActualizado.ImagenUrl;

        Guardar(datos);

        return true;
    }

    // Eliminar un personaje existente.
    public bool Eliminar(int id)
    {
        var datos = Leer();

        var existente = datos.FirstOrDefault(p => p.Id == id);

        if (existente == null)
        {
            return false;
        }

        datos.Remove(existente);

        Guardar(datos);

        return true;
    }
}
