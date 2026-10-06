using GranDT_ET12.Configuracion;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using GranDT_ET12.Service;
using Moq;
using Xunit;

namespace GranDT_ET12.Tests.Services
{
    public class EquipoServiceTests
    {
        private readonly Mock<IEquipoRepositorio> _equipoRepositorioMock;
        private readonly EquipoService _equipoService;

        public EquipoServiceTests()
        {
            // Inicialización de mocks y clase bajo prueba antes de cada test
            _equipoRepositorioMock = new Mock<IEquipoRepositorio>();
            _equipoService = new EquipoService(_equipoRepositorioMock.Object);
        }

        #region Casos de Creación (Crear)

        [Fact]
        public void Crear_EquipoValido_DevuelveIdGenerado()
        {
            // Arrange
            var nuevoEquipo = new Equipo { Nombre = "Boca Juniors" };
            var listaExistente = new List<Equipo>
            {
                new Equipo { Id = 1, Nombre = "River Plate" },
                new Equipo { Id = 2, Nombre = "Racing Club" }
            };

            _equipoRepositorioMock.Setup(repo => repo.ObtenerTodos())
                                  .Returns(listaExistente);

            _equipoRepositorioMock.Setup(repo => repo.InsertarEquipo(nuevoEquipo))
                                  .Returns(3);

            // Act
            int resultadoId = _equipoService.Crear(nuevoEquipo);

            // Assert
            Assert.Equal(3, resultadoId);
            _equipoRepositorioMock.Verify(repo => repo.ObtenerTodos(), Times.Once);
            _equipoRepositorioMock.Verify(repo => repo.InsertarEquipo(nuevoEquipo), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Crear_NombreVacioONulo_LanzaReglaNegocioException(string? nombreInvalido)
        {
            // Arrange
            var equipoInvalido = new Equipo { Nombre = nombreInvalido! };

            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() => _equipoService.Crear(equipoInvalido));

            Assert.Equal("El nombre del equipo es obligatorio.", excepcion.Message);
            _equipoRepositorioMock.Verify(repo => repo.ObtenerTodos(), Times.Never);
            _equipoRepositorioMock.Verify(repo => repo.InsertarEquipo(It.IsAny<Equipo>()), Times.Never);
        }

        [Fact]
        public void Crear_SuperaLimiteMaximoEquipos_LanzaReglaNegocioException()
        {
            // Arrange: Generamos la cantidad máxima permitida (Constantes.MaxEquipos = 32)
            var listaMaxima = new List<Equipo>();
            for (int i = 1; i <= Constantes.MaxEquipos; i++)
            {
                listaMaxima.Add(new Equipo { Id = i, Nombre = $"Equipo {i}" });
            }

            _equipoRepositorioMock.Setup(repo => repo.ObtenerTodos())
                                  .Returns(listaMaxima);

            var nuevoEquipo = new Equipo { Nombre = "Equipo 33" };

            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() => _equipoService.Crear(nuevoEquipo));

            Assert.Equal($"No puede haber mas de {Constantes.MaxEquipos} equipos.", excepcion.Message);
            _equipoRepositorioMock.Verify(repo => repo.ObtenerTodos(), Times.Once);
            _equipoRepositorioMock.Verify(repo => repo.InsertarEquipo(It.IsAny<Equipo>()), Times.Never);
        }

        [Fact]
        public void Crear_NombreDuplicadoMismoTexto_LanzaReglaNegocioException()
        {
            // Arrange: Simulamos que ya existe un equipo con el mismo nombre (ignora mayúsculas y minúsculas)
            var listaExistente = new List<Equipo>
            {
                new Equipo { Id = 1, Nombre = "San Lorenzo" }
            };

            _equipoRepositorioMock.Setup(repo => repo.ObtenerTodos())
                                  .Returns(listaExistente);

            var equipoDuplicado = new Equipo { Nombre = "san lorenzo" };

            // Act & Assert
            var excepcion = Assert.Throws<ReglaNegocioException>(() => _equipoService.Crear(equipoDuplicado));

            Assert.Equal("Ya existe un equipo con ese nombre.", excepcion.Message);
            _equipoRepositorioMock.Verify(repo => repo.ObtenerTodos(), Times.Once);
            _equipoRepositorioMock.Verify(repo => repo.InsertarEquipo(It.IsAny<Equipo>()), Times.Never);
        }

        #endregion

        #region Casos de Consulta (ObtenerPorId / ObtenerTodos)

        [Fact]
        public void ObtenerPorId_IdExistente_DevuelveEquipo()
        {
            // Arrange
            int idExistente = 5;
            var equipoEsperado = new Equipo { Id = idExistente, Nombre = "Independiente" };

            _equipoRepositorioMock.Setup(repo => repo.ObtenerPorId(idExistente))
                                  .Returns(equipoEsperado);

            // Act
            Equipo? resultado = _equipoService.ObtenerPorId(idExistente);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(idExistente, resultado.Id);
            Assert.Equal("Independiente", resultado.Nombre);
            _equipoRepositorioMock.Verify(repo => repo.ObtenerPorId(idExistente), Times.Once);
        }

        [Fact]
        public void ObtenerPorId_IdNoExistente_DevuelveNull()
        {
            // Arrange
            int idInexistente = 999;

            _equipoRepositorioMock.Setup(repo => repo.ObtenerPorId(idInexistente))
                                  .Returns((Equipo?)null);

            // Act
            Equipo? resultado = _equipoService.ObtenerPorId(idInexistente);

            // Assert
            Assert.Null(resultado);
            _equipoRepositorioMock.Verify(repo => repo.ObtenerPorId(idInexistente), Times.Once);
        }

        [Fact]
        public void ObtenerTodos_ExistenEquipos_DevuelveColeccionCompleta()
        {
            // Arrange
            var equiposEsperados = new List<Equipo>
            {
                new Equipo { Id = 1, Nombre = "Velez Sarsfield" },
                new Equipo { Id = 2, Nombre = "Huracan" },
                new Equipo { Id = 3, Nombre = "Estudiantes" }
            };

            _equipoRepositorioMock.Setup(repo => repo.ObtenerTodos())
                                  .Returns(equiposEsperados);

            // Act
            IEnumerable<Equipo> resultado = _equipoService.ObtenerTodos();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count());
            _equipoRepositorioMock.Verify(repo => repo.ObtenerTodos(), Times.Once);
        }

        #endregion
    }
}
