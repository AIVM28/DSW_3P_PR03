
using DSW_3P_PR02.Models;

namespace DSW_3P_PR02.Repositories;

public interface IPersonajeRepository
{
    List<Personaje> ObtenerTodos();

    Personaje? ObtenerPorId(int id);

    List<Personaje> ObtenerPorVideojuego(string nombre);

    List<Personaje> ObtenerPorNivel(int nivel);

    Personaje Agregar(Personaje nuevoPersonaje);

    bool Actualizar(int id, Personaje personajeActualizado);

    bool Eliminar(int id);
}
