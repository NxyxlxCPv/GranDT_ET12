using GranDT_ET12.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace GranDT_ET12.Repositorios.interfaces
{
    public interface IEquipoRepositorio
    {
        int InsertarEquipo(Equipo equipo);
        Equipo? ObtenerPorId(int id);
        IEnumerable<Equipo> ObtenerTodos();
        bool ExisteEquipo(int id);
    }
}
