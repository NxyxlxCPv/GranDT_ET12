using GranDT_ET12.Modelos;

namespace GranDT_ET12.Service.interfaces
{
    public interface IPuntuacionService
    {
        int Cargar(PuntuacionFecha puntuacion);
        IEnumerable<PuntuacionFecha> ObtenerPorFutbolista(int futbolistaId);
        IEnumerable<PuntuacionFecha> ObtenerPorFecha(int numeroFecha);
    }
}
