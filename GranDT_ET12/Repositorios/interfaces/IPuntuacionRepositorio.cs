using GranDT_ET12.Modelos;


namespace GranDT_ET12.Repositorios.interfaces
{
    public interface IPuntuacionRepositorio
    {
        int InsertarPuntuacion(PuntuacionFecha puntuacion);
        bool ExistePuntuacionEnFecha(int futbolistaId, int numeroFecha);
        IEnumerable<PuntuacionFecha> ObtenerPuntuacionesPorFutbolista(int futbolistaId);
        IEnumerable<PuntuacionFecha> ObtenerPuntuacionesPorFecha(int numeroFecha);
        decimal ObtenerPuntuacionPlantillaFecha(int plantillaId, int numeroFecha);
    }
}
