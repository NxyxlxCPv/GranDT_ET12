using GranDT_ET12.Modelos;

namespace GranDT_ET12.Service.interfaces
{
    public interface IPlantillaService
    {
        int Crear(int usuarioId);
        Plantilla? ObtenerPorId(int id);
        IEnumerable<Plantilla> ObtenerPorUsuario(int usuarioId);
        void AgregarJugador(int plantillaId, int futbolistaId, bool esTitular);
        bool QuitarJugador(int plantillaId, int futbolistaId);
        IEnumerable<DetallePlantilla> ObtenerDetalle(int plantillaId);
    }
}
