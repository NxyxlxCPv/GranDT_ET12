using GranDT_ET12.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace GranDT_ET12.Repositorios.interfaces
{
    public interface IFutbolistaRepositorio
    {
        int InsertarFutbolista(Futbolista futbolista);
        Futbolista? ObtenerPorId(int id);
        IEnumerable<Futbolista> ObtenerTodos();
        IEnumerable<Futbolista> ObtenerPorEquipo(int equipoId);
        IEnumerable<Futbolista> ObtenerPorPosicion(string tipo);
        int ContarFutbolistas();
    }
}
