using GranDT_ET12.Modelos;

namespace GranDT_ET12.Service.interfaces
{
    public interface IFutbolistaService
    {
        int Crear(Futbolista futbolista);
        Futbolista? ObtenerPorId(int id);
        IEnumerable<Futbolista> ObtenerTodos();
        IEnumerable<Futbolista> ObtenerPorEquipo(int equipoId);
        IEnumerable<Futbolista> ObtenerPorPosicion(string tipo);
    }
}
