using GranDT_ET12.Service.interfaces;
using GranDT_ET12.Configuracion;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;

namespace GranDT_ET12.Service
{
    public class PuntuacionService : IPuntuacionService
    {
        private IPuntuacionRepositorio _puntuacionRepositorio;
        private IFutbolistaRepositorio _futbolistaRepositorio;

        public PuntuacionService(IPuntuacionRepositorio puntuacionRepositorio, IFutbolistaRepositorio futbolistaRepositorio)
        {
            _puntuacionRepositorio = puntuacionRepositorio;
            _futbolistaRepositorio = futbolistaRepositorio;
        }

        public int Cargar(PuntuacionFecha puntuacion)
        {
            if (puntuacion.NumeroFecha < Constantes.FechaMinima || puntuacion.NumeroFecha > Constantes.FechaMaxima)
            {
                throw new ReglaNegocioException("La fecha tiene que estar entre " + Constantes.FechaMinima + " y " + Constantes.FechaMaxima);
            }
            //if (puntuacion.Puntaje < Constantes.PuntajeMinimo || puntuacion.Puntaje > Constantes.PuntajeMaximo)
            //{
            //    throw new ReglaNegocioException("El puntaje tiene que estar entre " + Constantes.PuntajeMinimo + " y " + Constantes.PuntajeMaximo);
            //}
            if (_futbolistaRepositorio.ObtenerPorId(puntuacion.FutbolistaId) == null)
            {
                throw new ReglaNegocioException("El futbolista indicado no existe.");
            }
            if (_puntuacionRepositorio.ExistePuntuacionEnFecha(puntuacion.FutbolistaId, puntuacion.NumeroFecha))
            {
                throw new ReglaNegocioException("El futbolista ya tiene una puntuacion en esa fecha.");
            }

            return _puntuacionRepositorio.InsertarPuntuacion(puntuacion);
        }

        public IEnumerable<PuntuacionFecha> ObtenerPorFutbolista(int futbolistaId)
        {
            return _puntuacionRepositorio.ObtenerPuntuacionesPorFutbolista(futbolistaId);
        }

        public IEnumerable<PuntuacionFecha> ObtenerPorFecha(int numeroFecha)
        {
            return _puntuacionRepositorio.ObtenerPuntuacionesPorFecha(numeroFecha);
        }
    }
}
