using GranDT_ET12.Service.interfaces;
using GranDT_ET12.Configuracion;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;

namespace GranDT_ET12.Service
{
    public class FutbolistaService : IFutbolistaService
    {
        private IFutbolistaRepositorio _futbolistaRepositorio;
        private IEquipoRepositorio _equipoRepositorio;

        public FutbolistaService(IFutbolistaRepositorio futbolistaRepositorio, IEquipoRepositorio equipoRepositorio)
        {
            _futbolistaRepositorio = futbolistaRepositorio;
            _equipoRepositorio = equipoRepositorio;
        }

        public int Crear(Futbolista futbolista)
        {
            if (futbolista.Nombre == "" || futbolista.Apellido == "")
            {
                throw new ReglaNegocioException("El nombre y el apellido son obligatorios.");
            }
            //if (futbolista.Cotizacion <= 0 || futbolista.Cotizacion > Constantes.CotizacionMaxima)
            //{
            //    throw new ReglaNegocioException("La cotizacion debe ser mayor a 0 y menor o igual a " + Constantes.CotizacionMaxima);
            //}
            if (_equipoRepositorio.ObtenerPorId(futbolista.EquipoId) == null)
            {
                throw new ReglaNegocioException("El equipo indicado no existe.");
            }
            //TODO traer cantidad
            int cantidadFutbolistas = _futbolistaRepositorio.ContarFutbolistas();
            if (cantidadFutbolistas >= Constantes.MaxFutbolistas)
            {
                throw new ReglaNegocioException("Se llego al maximo de futbolistas (" + Constantes.MaxFutbolistas + ").");
            }

            return _futbolistaRepositorio.InsertarFutbolista(futbolista);
        }

        public Futbolista? ObtenerPorId(int id)
        {
            return _futbolistaRepositorio.ObtenerPorId(id);
        }

        public IEnumerable<Futbolista> ObtenerTodos()
        {
            return _futbolistaRepositorio.ObtenerTodos();
        }

        public IEnumerable<Futbolista> ObtenerPorEquipo(int equipoId)
        {
            return _futbolistaRepositorio.ObtenerPorEquipo(equipoId);
        }

        public IEnumerable<Futbolista> ObtenerPorPosicion(string tipo)
        {
            // Revisa que el texto sea uno de los tipos del enum TipoFutbolista
            if (!Enum.IsDefined(typeof(TipoFutbolista), tipo))
            {
                throw new ReglaNegocioException("El tipo tiene que ser Arquero, Defensor, Mediocampista o Delantero.");
            }
            return _futbolistaRepositorio.ObtenerPorPosicion(tipo);
        }
    }
}
