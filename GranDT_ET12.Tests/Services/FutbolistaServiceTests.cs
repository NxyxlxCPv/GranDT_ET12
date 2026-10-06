using GranDT_ET12.Configuracion;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using GranDT_ET12.Service;
using Moq;
using Xunit;

namespace GranDT_ET12.Tests.Services
{
    public class FutbolistaServiceTests
    {
        private readonly Mock<IFutbolistaRepositorio> _futbolistaRepositorioMock;
        private readonly Mock<IEquipoRepositorio> _equipoRepositorioMock;
        private readonly FutbolistaService _futbolistaService;

        public FutbolistaServiceTests()
        {
            // Inicialización de mocks y clase bajo prueba antes de cada test
            _futbolistaRepositorioMock = new Mock<IFutbolistaRepositorio>();
            _equipoRepositorioMock = new Mock<IEquipoRepositorio>();

            _futbolistaService = new FutbolistaService(
                _futbolistaRepositorioMock.Object,
                _equipoRepositorioMock.Object
            );
        }

        #region Casos de Creación (Crear)

        [Fact]
        public void Crear_FutbolistaValido_DevuelveIdGenerado()
        {
            // Arrange
            var futbolista = new Futbolista
            {
                Nombre = "Lionel",
                Apellido = "Messi",
                EquipoId = 1,
                Tipo = TipoFutbolista.Delantero,
                Cotizacion = 50_000_000m
            };

            // Simular que el equipo existe
            _equipoRepositorioMock.Setup(repo => repo.ObtenerPorId(futbolista.EquipoId))
                                  .Returns(new Equipo { Id = 1, Nombre = "Inter Miami" });

            // Simular lista de futbolistas actual por debajo del límite
            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerTodos())
                                      .Returns(new List<Futbolista>());

            // Simular inserción exitosa
            _futbolistaRepositorioMock.Setup(repo => repo.InsertarFutbolista(futbolista))
                                      .Returns(10);

            // Act
            int resultadoId = _futbolistaService.Crear(futbolista);

            // Assert
            Assert.Equal(10, resultadoId);
            _equipoRepositorioMock.Verify(repo => repo.ObtenerPorId(futbolista.EquipoId), Times.Once);
            _futbolistaRepositorioMock.Verify(repo => repo.ObtenerTodos(), Times.Once);
            _futbolistaRepositorioMock.Verify(repo => repo.InsertarFutbolista(futbolista), Times.Once);
        }

        [Theory]
        [InlineData("", "Messi")]
        [InlineData("Lionel", "")]
        [InlineData("", "")]
        public void Crear_NombreOApellidoVacio_LanzaReglaNegocioException(string nombre, string apellido)
        {
            // Arrange
            var futbolistaInvalido = new Futbolista
            {
                Nombre = nombre,
                Apellido = apellido,
                EquipoId = 1
            };

            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() => _futbolistaService.Crear(futbolistaInvalido));

            Assert.Equal("El nombre y el apellido son obligatorios.", excepcion.Message);
            _equipoRepositorioMock.Verify(repo => repo.ObtenerPorId(It.IsAny<int>()), Times.Never);
            _futbolistaRepositorioMock.Verify(repo => repo.InsertarFutbolista(It.IsAny<Futbolista>()), Times.Never);
        }

        [Fact]
        public void Crear_EquipoNoExiste_LanzaReglaNegocioException()
        {
            // Arrange
            var futbolista = new Futbolista
            {
                Nombre = "Enzo",
                Apellido = "Fernandez",
                EquipoId = 999
            };

            // Simular que el equipo no existe (devuelve null)
            _equipoRepositorioMock.Setup(repo => repo.ObtenerPorId(futbolista.EquipoId))
                                  .Returns((Equipo?)null);

            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() => _futbolistaService.Crear(futbolista));

            Assert.Equal("El equipo indicado no existe.", excepcion.Message);
            _futbolistaRepositorioMock.Verify(repo => repo.ObtenerTodos(), Times.Never);
            _futbolistaRepositorioMock.Verify(repo => repo.InsertarFutbolista(It.IsAny<Futbolista>()), Times.Never);
        }

        [Fact]
        public void Crear_SuperaLimiteMaximoFutbolistas_LanzaReglaNegocioException()
        {
            // Arrange
            var futbolista = new Futbolista
            {
                Nombre = "Julian",
                Apellido = "Alvarez",
                EquipoId = 1
            };

            // Simular que el equipo existe
            _equipoRepositorioMock.Setup(repo => repo.ObtenerPorId(futbolista.EquipoId))
                                  .Returns(new Equipo { Id = 1, Nombre = "Atletico Madrid" });

            // Simular que ya se alcanzó el límite máximo de futbolistas (Constantes.MaxFutbolistas = 1500)
            var listaAlMaximo = Enumerable.Repeat(new Futbolista(), Constantes.MaxFutbolistas).ToList();
            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerTodos())
                                      .Returns(listaAlMaximo);

            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() => _futbolistaService.Crear(futbolista));

            Assert.Equal($"Se llego al maximo de futbolistas ({Constantes.MaxFutbolistas}).", excepcion.Message);
            _futbolistaRepositorioMock.Verify(repo => repo.InsertarFutbolista(It.IsAny<Futbolista>()), Times.Never);
        }

        #endregion

        #region Casos de Consulta y Filtrado

        [Fact]
        public void ObtenerPorId_FutbolistaExiste_DevuelveFutbolista()
        {
            // Arrange
            int idExistente = 7;
            var futbolistaEsperado = new Futbolista
            {
                Id = idExistente,
                Nombre = "Rodrigo",
                Apellido = "De Paul",
                EquipoId = 1,
                Tipo = TipoFutbolista.Mediocampista
            };

            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerPorId(idExistente))
                                      .Returns(futbolistaEsperado);

            // Act
            Futbolista? resultado = _futbolistaService.ObtenerPorId(idExistente);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(idExistente, resultado.Id);
            Assert.Equal("Rodrigo", resultado.Nombre);
            Assert.Equal("De Paul", resultado.Apellido);
            _futbolistaRepositorioMock.Verify(repo => repo.ObtenerPorId(idExistente), Times.Once);
        }

        [Fact]
        public void ObtenerPorId_FutbolistaNoExiste_DevuelveNull()
        {
            // Arrange
            int idInexistente = 999;
            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerPorId(idInexistente))
                                      .Returns((Futbolista?)null);

            // Act
            Futbolista? resultado = _futbolistaService.ObtenerPorId(idInexistente);

            // Assert
            Assert.Null(resultado);
            _futbolistaRepositorioMock.Verify(repo => repo.ObtenerPorId(idInexistente), Times.Once);
        }

        [Fact]
        public void ObtenerTodos_ExistenFutbolistas_DevuelveColeccionCompleta()
        {
            // Arrange
            var futbolistasEsperados = new List<Futbolista>
            {
                new Futbolista { Id = 1, Nombre = "Emiliano", Apellido = "Martinez", Tipo = TipoFutbolista.Arquero },
                new Futbolista { Id = 2, Nombre = "Cristian", Apellido = "Romero", Tipo = TipoFutbolista.Defensor }
            };

            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerTodos())
                                      .Returns(futbolistasEsperados);

            // Act
            IEnumerable<Futbolista> resultado = _futbolistaService.ObtenerTodos();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            _futbolistaRepositorioMock.Verify(repo => repo.ObtenerTodos(), Times.Once);
        }

        [Fact]
        public void ObtenerPorEquipo_ExistenFutbolistas_DevuelveFutbolistasDelEquipo()
        {
            // Arrange
            int equipoId = 2;
            var futbolistasDelEquipo = new List<Futbolista>
            {
                new Futbolista { Id = 1, Nombre = "Lautaro", Apellido = "Martinez", EquipoId = equipoId },
                new Futbolista { Id = 2, Nombre = "Alexis", Apellido = "Mac Allister", EquipoId = equipoId }
            };

            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerPorEquipo(equipoId))
                                      .Returns(futbolistasDelEquipo);

            // Act
            IEnumerable<Futbolista> resultado = _futbolistaService.ObtenerPorEquipo(equipoId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.All(resultado, f => Assert.Equal(equipoId, f.EquipoId));
            _futbolistaRepositorioMock.Verify(repo => repo.ObtenerPorEquipo(equipoId), Times.Once);
        }

        [Theory]
        [InlineData("Arquero")]
        [InlineData("Defensor")]
        [InlineData("Mediocampista")]
        [InlineData("Delantero")]
        public void ObtenerPorPosicion_PosicionValida_DevuelveFutbolistasPorTipo(string tipoValido)
        {
            // Arrange
            var futbolistasPorPosicion = new List<Futbolista>
            {
                new Futbolista { Id = 1, Nombre = "Jugador 1", Apellido = "Prueba" }
            };

            _futbolistaRepositorioMock.Setup(repo => repo.ObtenerPorPosicion(tipoValido))
                                      .Returns(futbolistasPorPosicion);

            // Act
            IEnumerable<Futbolista> resultado = _futbolistaService.ObtenerPorPosicion(tipoValido);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            _futbolistaRepositorioMock.Verify(repo => repo.ObtenerPorPosicion(tipoValido), Times.Once);
        }

        [Theory]
        [InlineData("Invalido")]
        [InlineData("Carrilero")]
        [InlineData("arquero")] // Notar: Enum.IsDefined es sensible a mayúsculas/minúsculas
        [InlineData("")]
        public void ObtenerPorPosicion_PosicionInvalida_LanzaReglaNegocioException(string tipoInvalido)
        {
            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() => _futbolistaService.ObtenerPorPosicion(tipoInvalido));

            Assert.Equal("El tipo tiene que ser Arquero, Defensor, Mediocampista o Delantero.", excepcion.Message);
            _futbolistaRepositorioMock.Verify(repo => repo.ObtenerPorPosicion(It.IsAny<string>()), Times.Never);
        }

        #endregion
    }
}
