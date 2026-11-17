using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using GranDT_ET12.Service;
using Moq;
using Xunit;

namespace GranDT_ET12.Tests.Services
{
    public class PuntuacionServiceTests
    {
        private readonly Mock<IPuntuacionRepositorio> _puntuacionRepositorioMock;
        private readonly Mock<IFutbolistaRepositorio> _futbolistaRepositorioMock;
        private readonly PuntuacionService _puntuacionService;

        public PuntuacionServiceTests()
        {
            _puntuacionRepositorioMock = new Mock<IPuntuacionRepositorio>();
            _futbolistaRepositorioMock = new Mock<IFutbolistaRepositorio>();
            _puntuacionService = new PuntuacionService(
                _puntuacionRepositorioMock.Object,
                _futbolistaRepositorioMock.Object);
        }

        [Fact]
        public void T1_Cargar_DatosValidos_DevuelveIdGenerado()
        {
            var puntuacion = new PuntuacionFecha { FutbolistaId = 7, NumeroFecha = 4, Puntaje = 8m };
            _futbolistaRepositorioMock.Setup(r => r.ObtenerPorId(7)).Returns(new Futbolista { Id = 7 });
            _puntuacionRepositorioMock.Setup(r => r.ExistePuntuacionEnFecha(7, 4)).Returns(false);
            _puntuacionRepositorioMock.Setup(r => r.InsertarPuntuacion(puntuacion)).Returns(3);

            int resultado = _puntuacionService.Cargar(puntuacion);

            Assert.Equal(3, resultado);
            _puntuacionRepositorioMock.Verify(r => r.InsertarPuntuacion(puntuacion), Times.Once);
        }

        [Fact]
        public void T2_Cargar_FutbolistaYaPuntuadoEnFecha_LanzaReglaNegocioException()
        {
            var puntuacion = new PuntuacionFecha { FutbolistaId = 7, NumeroFecha = 4, Puntaje = 8m };
            _futbolistaRepositorioMock.Setup(r => r.ObtenerPorId(7)).Returns(new Futbolista { Id = 7 });
            _puntuacionRepositorioMock.Setup(r => r.ExistePuntuacionEnFecha(7, 4)).Returns(true);

            var excepcion = Assert.Throws<ReglaNegocioException>(() => _puntuacionService.Cargar(puntuacion));

            Assert.Equal("El futbolista ya tiene una puntuacion en esa fecha.", excepcion.Message);
            _puntuacionRepositorioMock.Verify(r => r.InsertarPuntuacion(It.IsAny<PuntuacionFecha>()), Times.Never);
        }

        [Fact]
        public void T3_Cargar_FechaFueraDeRango_LanzaReglaNegocioException()
        {
            var puntuacion = new PuntuacionFecha { FutbolistaId = 7, NumeroFecha = 50, Puntaje = 8m };

            var excepcion = Assert.Throws<ReglaNegocioException>(() => _puntuacionService.Cargar(puntuacion));

            Assert.Equal("La fecha tiene que estar entre 1 y 49", excepcion.Message);
            _futbolistaRepositorioMock.Verify(r => r.ObtenerPorId(It.IsAny<int>()), Times.Never);
            _puntuacionRepositorioMock.Verify(r => r.InsertarPuntuacion(It.IsAny<PuntuacionFecha>()), Times.Never);
        }

        [Fact]
        public void T4_Cargar_FutbolistaNoExiste_LanzaReglaNegocioException()
        {
            var puntuacion = new PuntuacionFecha { FutbolistaId = 999, NumeroFecha = 4, Puntaje = 8m };
            _futbolistaRepositorioMock.Setup(r => r.ObtenerPorId(999)).Returns((Futbolista?)null);

            var excepcion = Assert.Throws<ReglaNegocioException>(() => _puntuacionService.Cargar(puntuacion));

            Assert.Equal("El futbolista indicado no existe.", excepcion.Message);
            _puntuacionRepositorioMock.Verify(r => r.ExistePuntuacionEnFecha(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _puntuacionRepositorioMock.Verify(r => r.InsertarPuntuacion(It.IsAny<PuntuacionFecha>()), Times.Never);
        }

        [Fact]
        public void T5_ObtenerPorFutbolista_DevuelvePuntuacionesDelFutbolista()
        {
            var esperadas = new List<PuntuacionFecha>
            {
                new PuntuacionFecha { Id = 1, FutbolistaId = 7, NumeroFecha = 4, Puntaje = 8m }
            };
            _puntuacionRepositorioMock.Setup(r => r.ObtenerPuntuacionesPorFutbolista(7)).Returns(esperadas);

            var resultado = _puntuacionService.ObtenerPorFutbolista(7);

            Assert.Same(esperadas, resultado);
            _puntuacionRepositorioMock.Verify(r => r.ObtenerPuntuacionesPorFutbolista(7), Times.Once);
        }

        [Fact]
        public void T6_ObtenerPorFecha_DevuelvePuntuacionesDeLaFecha()
        {
            var esperadas = new List<PuntuacionFecha>
            {
                new PuntuacionFecha { Id = 2, FutbolistaId = 7, NumeroFecha = 4, Puntaje = 8m }
            };
            _puntuacionRepositorioMock.Setup(r => r.ObtenerPuntuacionesPorFecha(4)).Returns(esperadas);

            var resultado = _puntuacionService.ObtenerPorFecha(4);

            Assert.Same(esperadas, resultado);
            _puntuacionRepositorioMock.Verify(r => r.ObtenerPuntuacionesPorFecha(4), Times.Once);
        }
    }
}
