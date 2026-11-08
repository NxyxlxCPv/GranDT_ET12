using GranDT_ET12.Service.interfaces;
using GranDT_ET12.Configuracion;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;

namespace GranDT_ET12.Service
{
    public class PlantillaService : IPlantillaService
    {
        private readonly IPlantillaRepositorio _plantillaRepositorio;
        private readonly IDetallePlantillaRepositorio _detalleRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IFutbolistaRepositorio _futbolistaRepositorio;

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

        // Muchos a muchos: agrega un futbolista a la plantilla validando reglas de negocio
        public void AgregarJugador(int plantillaId, int futbolistaId, bool esTitular)
        {
            Plantilla? plantilla = _plantillaRepositorio.ObtenerPorId(plantillaId);
            if (plantilla == null)
            {
                throw new ReglaNegocioException("La plantilla indicada no existe.");
            }

            Futbolista? futbolista = _futbolistaRepositorio.ObtenerPorId(futbolistaId);
            if (futbolista == null)
            {
                throw new ReglaNegocioException("El futbolista indicado no existe.");
            }

            var detalles = _detalleRepositorio.ObtenerPorPlantillaId(plantillaId)?.ToList() ?? new List<DetallePlantilla>();

            if (detalles.Any(d => d.FutbolistaId == futbolistaId))
            {
                throw new ReglaNegocioException("El futbolista ya se encuentra en la plantilla.");
            }

            int limiteJugadores = plantilla.CantidadMaximaJugadores > 0 ? plantilla.CantidadMaximaJugadores : Constantes.MaxJugadoresPlantilla;
            if (detalles.Count >= limiteJugadores)
            {
                throw new ReglaNegocioException("La plantilla ya tiene el maximo de " + limiteJugadores + " jugadores.");
            }

            decimal presupuestoMaximo = plantilla.PresupuestoMaximo > 0 ? plantilla.PresupuestoMaximo : Constantes.PresupuestoMaximoPlantilla;
            decimal gastoActual = 0m;
            foreach (var d in detalles)
            {
                var f = _futbolistaRepositorio.ObtenerPorId(d.FutbolistaId);
                if (f != null)
                {
                    gastoActual += f.Cotizacion;
                }
            }

            if (gastoActual + futbolista.Cotizacion > presupuestoMaximo)
            {
                throw new ReglaNegocioException("El presupuesto maximo de la plantilla ha sido superado.");
            }

            DetallePlantilla nuevo = new DetallePlantilla
            {
                PlantillaId = plantillaId,
                FutbolistaId = futbolistaId,
                EsTitular = esTitular
            };
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
