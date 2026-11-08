using GranDT_ET12.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace GranDT_ET12.Repositorios.interfaces
{
    public interface IDetallePlantillaRepositorio
    {
        bool AgregarJugador(DetallePlantilla detalle);
        bool EliminarJugador(int plantillaId, int futbolistaId);
        IEnumerable<DetallePlantilla> ObtenerPorPlantillaId(int plantillaId);
        int ContarJugadoresEnPlantilla(int plantillaId);
    }
}
