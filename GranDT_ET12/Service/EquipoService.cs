using System;
using GranDT_ET12.Configuracion;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;

namespace GranDT_ET12.Service
{
    public class EquipoService
    {
        private IEquipoRepositorio _equipoRepositorio;

        public EquipoService(IEquipoRepositorio equipoRepositorio)
        {
            _equipoRepositorio = equipoRepositorio;
        }

        public virtual int Crear(Equipo equipo)
        {
            if (string.IsNullOrEmpty(equipo.Nombre))
               throw new ReglaNegocioException("El nombre del equipo es obligatorio.");
            
            List<Equipo> equipos = _equipoRepositorio.ObtenerTodos().ToList();
            if (equipos.Count >= Constantes.MaxEquipos)
            {
                throw new ReglaNegocioException("No puede haber mas de " + Constantes.MaxEquipos + " equipos.");
            }

            foreach (Equipo e in equipos)
            {
                if (e.Nombre.ToLower() == equipo.Nombre.ToLower())
                {
                    throw new ReglaNegocioException("Ya existe un equipo con ese nombre.");
                }
            }

            if (_equipoRepositorio.ExisteEquipo(equipo.Id))
            {
                throw new InvalidOperationException("Ya existe un equipo registrado con el ID especificado.");
            }

            return _equipoRepositorio.InsertarEquipo(equipo);
        }

        public virtual Equipo? ObtenerPorId(int id) => _equipoRepositorio.ObtenerPorId(id);

        public virtual IEnumerable<Equipo> ObtenerTodos()
        {
            return _equipoRepositorio.ObtenerTodos();
        }
    }
}
