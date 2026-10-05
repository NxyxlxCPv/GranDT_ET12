using GranDT_ET12.Service.interfaces;
using GranDT_ET12.Configuracion;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;

namespace GranDT_ET12.Service
{
    public class EquipoService : IEquipoService
    {
        private IEquipoRepositorio _equipoRepositorio;

        public EquipoService(IEquipoRepositorio equipoRepositorio)
        {
            _equipoRepositorio = equipoRepositorio;
        }

        public int Crear(Equipo equipo)
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

            return _equipoRepositorio.InsertarEquipo(equipo);
        }

        public Equipo? ObtenerPorId(int id) => _equipoRepositorio.ObtenerPorId(id);

        public IEnumerable<Equipo> ObtenerTodos()
        {
            return _equipoRepositorio.ObtenerTodos();
        }
    }
}
