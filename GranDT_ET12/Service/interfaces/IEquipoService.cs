using GranDT_ET12.Modelos;

namespace GranDT_ET12.Service.interfaces
{
    public interface IEquipoService
    {
        int Crear(Equipo equipo);
        Equipo? ObtenerPorId(int id);
        IEnumerable<Equipo> ObtenerTodos();
    }
}
