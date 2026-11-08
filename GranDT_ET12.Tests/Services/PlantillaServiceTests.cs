using GranDT_ET12.Configuracion;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using GranDT_ET12.Service;
using Moq;
using Xunit;

namespace GranDT_ET12.Tests.Services
{
    public class PlantillaServiceTests
    {
        private readonly Mock<IPlantillaRepositorio> _plantillaRepositorioMock;
        private readonly Mock<IDetallePlantillaRepositorio> _detalleRepositorioMock;
        private readonly Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private readonly Mock<IFutbolistaRepositorio> _futbolistaRepositorioMock;
        private readonly PlantillaService _plantillaService;

        public PlantillaServiceTests()
        {
            _plantillaRepositorioMock = new Mock<IPlantillaRepositorio>();
            _detalleRepositorioMock = new Mock<IDetallePlantillaRepositorio>();
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _futbolistaRepositorioMock = new Mock<IFutbolistaRepositorio>();

            _plantillaService = new PlantillaService(
                _plantillaRepositorioMock.Object,
                _detalleRepositorioMock.Object,
                _usuarioRepositorioMock.Object,
                _futbolistaRepositorioMock.Object
            );
        }

        #region Reglas de Negocio - Agregar Jugador

        [Fact]
        public void AgregarJugador_FutbolistaYaExisteEnPlantilla_LanzaReglaNegocioException()
        {
            // Arrange
            int plantillaId = 1;
            int futbolistaId = 10;
            bool esTitular = true;

            var plantilla = new Plantilla
            {
                Id = plantillaId,
                UsuarioId = 1,
                PresupuestoMaximo = Constantes.PresupuestoMaximoPlantilla,
                CantidadMaximaJugadores = Constantes.MaxJugadoresPlantilla
            };

            var futbolista = new Futbolista
            {
                Id = futbolistaId,
                Nombre = "Lionel",
                Apellido = "Messi",
                Cotizacion = 50_000_000m
            };

            var detallesExistentes = new List<DetallePlantilla>
            {
                new DetallePlantilla { PlantillaId = plantillaId, FutbolistaId = futbolistaId, EsTitular = true }
            };

            _plantillaRepositorioMock.Setup(repo => repo.ObtenerPorId(plantillaId))
                                     .Returns(plantilla);
            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerPorId(futbolistaId))
                                      .Returns(futbolista);
            _detalleRepositorioMock.Setup(repo => repo.ObtenerPorPlantillaId(plantillaId))
                                   .Returns(detallesExistentes);

            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() =>
                _plantillaService.AgregarJugador(plantillaId, futbolistaId, esTitular));

            Assert.Equal("El futbolista ya se encuentra en la plantilla.", excepcion.Message);
            _detalleRepositorioMock.Verify(repo => repo.AgregarJugador(It.IsAny<DetallePlantilla>()), Times.Never);
        }

        [Fact]
        public void AgregarJugador_PlantillaNoExiste_LanzaReglaNegocioException()
        {
            // Arrange
            int plantillaId = 999;
            int futbolistaId = 5;

            _plantillaRepositorioMock.Setup(repo => repo.ObtenerPorId(plantillaId))
                                     .Returns((Plantilla?)null);

            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() =>
                _plantillaService.AgregarJugador(plantillaId, futbolistaId, true));

            Assert.Equal("La plantilla indicada no existe.", excepcion.Message);
            _futbolistaRepositorioMock.Verify(repo => repo.ObtenerPorId(It.IsAny<int>()), Times.Never);
            _detalleRepositorioMock.Verify(repo => repo.AgregarJugador(It.IsAny<DetallePlantilla>()), Times.Never);
        }

        [Fact]
        public void AgregarJugador_FutbolistaNoExiste_LanzaReglaNegocioException()
        {
            // Arrange
            int plantillaId = 1;
            int futbolistaId = 999;

            var plantilla = new Plantilla { Id = plantillaId, PresupuestoMaximo = 500_000_000m };

            _plantillaRepositorioMock.Setup(repo => repo.ObtenerPorId(plantillaId))
                                     .Returns(plantilla);
            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerPorId(futbolistaId))
                                      .Returns((Futbolista?)null);

            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() =>
                _plantillaService.AgregarJugador(plantillaId, futbolistaId, false));

            Assert.Equal("El futbolista indicado no existe.", excepcion.Message);
            _detalleRepositorioMock.Verify(repo => repo.AgregarJugador(It.IsAny<DetallePlantilla>()), Times.Never);
        }

        [Fact]
        public void AgregarJugador_SuperaLimiteMaximoJugadores_LanzaReglaNegocioException()
        {
            // Arrange
            int plantillaId = 1;
            int nuevoFutbolistaId = 21;

            var plantilla = new Plantilla
            {
                Id = plantillaId,
                PresupuestoMaximo = Constantes.PresupuestoMaximoPlantilla,
                CantidadMaximaJugadores = Constantes.MaxJugadoresPlantilla
            };

            var nuevoFutbolista = new Futbolista
            {
                Id = nuevoFutbolistaId,
                Nombre = "Angel",
                Apellido = "Di Maria",
                Cotizacion = 10_000_000m
            };

            // Simular que ya se alcanzaron 20 jugadores distintos
            var detallesMaximos = new List<DetallePlantilla>();
            for (int i = 1; i <= Constantes.MaxJugadoresPlantilla; i++)
            {
                detallesMaximos.Add(new DetallePlantilla { PlantillaId = plantillaId, FutbolistaId = i });
            }

            _plantillaRepositorioMock.Setup(repo => repo.ObtenerPorId(plantillaId))
                                     .Returns(plantilla);
            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerPorId(nuevoFutbolistaId))
                                      .Returns(nuevoFutbolista);
            _detalleRepositorioMock.Setup(repo => repo.ObtenerPorPlantillaId(plantillaId))
                                   .Returns(detallesMaximos);

            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() =>
                _plantillaService.AgregarJugador(plantillaId, nuevoFutbolistaId, false));

            Assert.Equal($"La plantilla ya tiene el maximo de {Constantes.MaxJugadoresPlantilla} jugadores.", excepcion.Message);
            _detalleRepositorioMock.Verify(repo => repo.AgregarJugador(It.IsAny<DetallePlantilla>()), Times.Never);
        }

        [Fact]
        public void AgregarJugador_SuperaPresupuestoMaximo_LanzaReglaNegocioException()
        {
            // Arrange
            int plantillaId = 1;
            int nuevoFutbolistaId = 2;

            var plantilla = new Plantilla
            {
                Id = plantillaId,
                PresupuestoMaximo = 500_000_000m,
                CantidadMaximaJugadores = 20
            };

            var futbolistaExistente = new Futbolista
            {
                Id = 1,
                Nombre = "Kylian",
                Apellido = "Mbappe",
                Cotizacion = 300_000_000m
            };

            var nuevoFutbolista = new Futbolista
            {
                Id = nuevoFutbolistaId,
                Nombre = "Erling",
                Apellido = "Haaland",
                Cotizacion = 250_000_000m // 300M + 250M = 550M > 500M
            };

            var detalles = new List<DetallePlantilla>
            {
                new DetallePlantilla { PlantillaId = plantillaId, FutbolistaId = 1 }
            };

            _plantillaRepositorioMock.Setup(repo => repo.ObtenerPorId(plantillaId))
                                     .Returns(plantilla);
            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerPorId(nuevoFutbolistaId))
                                      .Returns(nuevoFutbolista);
            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerPorId(1))
                                      .Returns(futbolistaExistente);
            _detalleRepositorioMock.Setup(repo => repo.ObtenerPorPlantillaId(plantillaId))
                                   .Returns(detalles);

            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() =>
                _plantillaService.AgregarJugador(plantillaId, nuevoFutbolistaId, true));

            Assert.Equal("El presupuesto maximo de la plantilla ha sido superado.", excepcion.Message);
            _detalleRepositorioMock.Verify(repo => repo.AgregarJugador(It.IsAny<DetallePlantilla>()), Times.Never);
        }

        [Fact]
        public void AgregarJugador_DatosValidos_AgregaExitosamente()
        {
            // Arrange
            int plantillaId = 1;
            int nuevoFutbolistaId = 2;

            var plantilla = new Plantilla
            {
                Id = plantillaId,
                PresupuestoMaximo = 500_000_000m,
                CantidadMaximaJugadores = 20
            };

            var nuevoFutbolista = new Futbolista
            {
                Id = nuevoFutbolistaId,
                Nombre = "Emiliano",
                Apellido = "Martinez",
                Cotizacion = 25_000_000m
            };

            _plantillaRepositorioMock.Setup(repo => repo.ObtenerPorId(plantillaId))
                                     .Returns(plantilla);
            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerPorId(nuevoFutbolistaId))
                                      .Returns(nuevoFutbolista);
            _detalleRepositorioMock.Setup(repo => repo.ObtenerPorPlantillaId(plantillaId))
                                   .Returns(new List<DetallePlantilla>());

            // Act
            _plantillaService.AgregarJugador(plantillaId, nuevoFutbolistaId, true);

            // Assert
            _detalleRepositorioMock.Verify(repo => repo.AgregarJugador(It.Is<DetallePlantilla>(d =>
                d.PlantillaId == plantillaId &&
                d.FutbolistaId == nuevoFutbolistaId &&
                d.EsTitular == true
            )), Times.Once);
        }

        #endregion

        #region Casos de Creación y Consulta

        [Fact]
        public void Crear_UsuarioExiste_DevuelveIdNuevaPlantilla()
        {
            // Arrange
            int usuarioId = 3;
            _usuarioRepositorioMock.Setup(repo => repo.ObtenerPorId(usuarioId))
                                   .Returns(new Usuario { Id = usuarioId, Nombre = "Juan" });

            _plantillaRepositorioMock.Setup(repo => repo.CrearPlantilla(It.IsAny<Plantilla>()))
                                     .Returns(10);

            // Act
            int nuevoId = _plantillaService.Crear(usuarioId);

            // Assert
            Assert.Equal(10, nuevoId);
            _plantillaRepositorioMock.Verify(repo => repo.CrearPlantilla(It.Is<Plantilla>(p =>
                p.UsuarioId == usuarioId &&
                p.PresupuestoMaximo == Constantes.PresupuestoMaximoPlantilla &&
                p.CantidadMaximaJugadores == Constantes.MaxJugadoresPlantilla
            )), Times.Once);
        }

        [Fact]
        public void Crear_UsuarioNoExiste_LanzaReglaNegocioException()
        {
            // Arrange
            int usuarioId = 999;
            _usuarioRepositorioMock.Setup(repo => repo.ObtenerPorId(usuarioId))
                                   .Returns((Usuario?)null);

            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() => _plantillaService.Crear(usuarioId));

            Assert.Equal("El usuario indicado no existe.", excepcion.Message);
            _plantillaRepositorioMock.Verify(repo => repo.CrearPlantilla(It.IsAny<Plantilla>()), Times.Never);
        }

        [Fact]
        public void QuitarJugador_LlamaRepositorioCorrectamente()
        {
            // Arrange
            int plantillaId = 1;
            int futbolistaId = 7;
            _detalleRepositorioMock.Setup(repo => repo.EliminarJugador(plantillaId, futbolistaId))
                                   .Returns(true);

            // Act
            bool resultado = _plantillaService.QuitarJugador(plantillaId, futbolistaId);

            // Assert
            Assert.True(resultado);
            _detalleRepositorioMock.Verify(repo => repo.EliminarJugador(plantillaId, futbolistaId), Times.Once);
        }

        [Fact]
        public void ObtenerDetalle_DevuelveListaDeDetalles()
        {
            // Arrange
            int plantillaId = 1;
            var detallesEsperados = new List<DetallePlantilla>
            {
                new DetallePlantilla { PlantillaId = plantillaId, FutbolistaId = 1, EsTitular = true }
            };
            _detalleRepositorioMock.Setup(repo => repo.ObtenerPorPlantillaId(plantillaId))
                                   .Returns(detallesEsperados);

            // Act
            var resultado = _plantillaService.ObtenerDetalle(plantillaId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            _detalleRepositorioMock.Verify(repo => repo.ObtenerPorPlantillaId(plantillaId), Times.Once);
        }

        #endregion
    }
}
