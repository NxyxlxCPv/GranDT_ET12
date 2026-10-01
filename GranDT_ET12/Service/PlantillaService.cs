using GranDT_ET12.Service.interfaces;
using GranDT_ET12.Configuracion;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;

namespace GranDT_ET12.Service
{
    public class PlantillaService : IPlantillaService
    {
        private IPlantillaRepositorio _plantillaRepositorio;
        private IDetallePlantillaRepositorio _detalleRepositorio;
        private IUsuarioRepositorio _usuarioRepositorio;
        private IFutbolistaRepositorio _futbolistaRepositorio;

        public PlantillaService(
            IPlantillaRepositorio plantillaRepositorio,
            IDetallePlantillaRepositorio detalleRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IFutbolistaRepositorio futbolistaRepositorio)
        {
            _plantillaRepositorio = plantillaRepositorio;
            _detalleRepositorio = detalleRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _futbolistaRepositorio = futbolistaRepositorio;
        }

        // Crea una plantilla vacia para el usuario. Devuelve el Id nuevo.
        public int Crear(int usuarioId)
        {
            if (_usuarioRepositorio.ObtenerPorId(usuarioId) == null)
            {
                throw new ReglaNegocioException("El usuario indicado no existe.");
            }

            Plantilla plantilla = new Plantilla();
            plantilla.UsuarioId = usuarioId;
            plantilla.PresupuestoMaximo = Constantes.PresupuestoMaximoPlantilla;
            plantilla.CantidadMaximaJugadores = Constantes.MaxJugadoresPlantilla;
            return _plantillaRepositorio.CrearPlantilla(plantilla);
        }

        public Plantilla? ObtenerPorId(int id)
        {
            return _plantillaRepositorio.ObtenerPorId(id);
        }

        public IEnumerable<Plantilla> ObtenerPorUsuario(int usuarioId)
        {
            return _plantillaRepositorio.ObtenerPorUsuarioId(usuarioId);
        }

        // Muchos a muchos: agrega un futbolista a la plantilla
        // (pendiente: validar el presupuesto y los tipos de jugador de los titulares)
        public void AgregarJugador(int plantillaId, int futbolistaId, bool esTitular)
        {
            Plantilla? plantilla = _plantillaRepositorio.ObtenerPorId(plantillaId);
            if (plantilla == null)
            {
                throw new ReglaNegocioException("La plantilla indicada no existe.");
            }
            if (_futbolistaRepositorio.ObtenerPorId(futbolistaId) == null)
            {
                throw new ReglaNegocioException("El futbolista indicado no existe.");
            }

            foreach (DetallePlantilla detalle in _detalleRepositorio.ObtenerPorPlantillaId(plantillaId))
            {
                if (detalle.FutbolistaId == futbolistaId)
                {
                    throw new ReglaNegocioException("El futbolista ya esta en la plantilla.");
                }
            }

            int cantidadJugadores = _detalleRepositorio.ContarJugadoresEnPlantilla(plantillaId);
            if (cantidadJugadores >= plantilla.CantidadMaximaJugadores)
            {
                throw new ReglaNegocioException("La plantilla ya tiene el maximo de " + plantilla.CantidadMaximaJugadores + " jugadores.");
            }

            DetallePlantilla nuevo = new DetallePlantilla();
            nuevo.PlantillaId = plantillaId;
            nuevo.FutbolistaId = futbolistaId;
            nuevo.EsTitular = esTitular;
            _detalleRepositorio.AgregarJugador(nuevo);
        }

        public bool QuitarJugador(int plantillaId, int futbolistaId)
        {
            return _detalleRepositorio.EliminarJugador(plantillaId, futbolistaId);
        }

        public IEnumerable<DetallePlantilla> ObtenerDetalle(int plantillaId)
        {
            return _detalleRepositorio.ObtenerPorPlantillaId(plantillaId);
        }
    }
}
