using GranDT_ET12.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace GranDT_ET12.Repositorios.interfaces
{
    public interface IPlantillaRepositorio
    {
        int CrearPlantilla(Plantilla plantilla);
        IEnumerable<Plantilla> ObtenerPorUsuarioId(int usuarioId);
        Plantilla? ObtenerPorId(int id);
        decimal ObtenerGastoTotalPlantilla(int plantillaId);
    }
}
