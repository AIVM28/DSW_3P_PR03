
using DSW_3P_PR02.Models;
using DSW_3P_PR02.Repositories;

namespace DSW_3P_PR02.Services;

public class PersonajeService : IPersonajeService
{
    private readonly IPersonajeRepository _repository;

    public PersonajeService(IPersonajeRepository repository)
    {
        _repository = repository;
    }

    // Validar las reglas de negocio de un personaje.
    private static void Validar(Personaje personaje)
    {
        if (string.IsNullOrWhiteSpace(personaje.Nombre))
        {
            throw new ArgumentException(
                "El nombre del personaje es obligatorio."
            );
        }

        if (string.IsNullOrWhiteSpace(personaje.Videojuego))
        {
            throw new ArgumentException(
                "El videojuego es obligatorio."
            );
        }

        if (string.IsNullOrWhiteSpace(personaje.Clase))
        {
            throw new ArgumentException(
                "La clase del personaje es obligatoria."
            );
        }

        if (personaje.Nivel < 1 || personaje.Nivel > 100)
        {
            throw new ArgumentException(
                "El nivel debe estar entre 1 y 100."
            );
        }

        if (personaje.Poder <= 0)
        {
            throw new ArgumentException(
                "El poder debe ser mayor que cero."
            );
        }

        if (
            !Uri.TryCreate(
                personaje.ImagenUrl,
                UriKind.Absolute,
                out Uri? imagenUri
            )
            || (imagenUri.Scheme != Uri.UriSchemeHttp
                && imagenUri.Scheme != Uri.UriSchemeHttps)
        )
        {
            throw new ArgumentException(
                "La URL de la imagen debe ser absoluta y utilizar HTTP o HTTPS."
            );
        }
    }

    // Obtener todos los personajes.
    public List<Personaje> ObtenerTodos()
    {
        return _repository.ObtenerTodos();
    }

    // Obtener un personaje por su identificador.
    public Personaje? ObtenerPorId(int id)
    {
        return _repository.ObtenerPorId(id);
    }

    // Obtener personajes por videojuego.
    public List<Personaje> ObtenerPorVideojuego(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException(
                "El nombre del videojuego es obligatorio."
            );
        }

        return _repository.ObtenerPorVideojuego(nombre);
    }

    // Obtener personajes con nivel igual o superior.
    public List<Personaje> ObtenerPorNivel(int nivel)
    {
        if (nivel < 1 || nivel > 100)
        {
            throw new ArgumentException(
                "El nivel debe estar entre 1 y 100."
            );
        }

        return _repository.ObtenerPorNivel(nivel);
    }

    // Registrar un nuevo personaje.
    public Personaje Agregar(Personaje nuevoPersonaje)
    {
        Validar(nuevoPersonaje);

        // Todos los personajes nuevos comienzan disponibles.
        nuevoPersonaje.Disponible = true;

        return _repository.Agregar(nuevoPersonaje);
    }

    // Actualizar un personaje existente.
    public bool Actualizar(int id, Personaje personajeActualizado)
    {
        if (_repository.ObtenerPorId(id) == null)
        {
            return false;
        }

        Validar(personajeActualizado);

        // El identificador de la URL prevalece sobre el del cuerpo.
        personajeActualizado.Id = id;

        return _repository.Actualizar(id, personajeActualizado);
    }

    // Eliminar un personaje existente.
    public bool Eliminar(int id)
    {
        return _repository.Eliminar(id);
    }
}
