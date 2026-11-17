using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using GranDT_ET12.Service;
using GranDT_ET12API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GranDT_ET12.Tests.Controllers
{
    public class PuntuacionesControllerTests
    {
        private readonly Mock<PuntuacionService> _servicioFalso;
        private readonly PuntuacionesController _controlador;

        public PuntuacionesControllerTests()
        {
            _servicioFalso = new Mock<PuntuacionService>(
                Mock.Of<IPuntuacionRepositorio>(),
                Mock.Of<IFutbolistaRepositorio>());
            _controlador = new PuntuacionesController(_servicioFalso.Object);
        }

        [Fact]
        public void T1_Cargar_DatosValidos_DevuelveOkConElId()
        {
            _servicioFalso.Setup(s => s.Cargar(It.IsAny<PuntuacionFecha>())).Returns(5);
            var puntuacion = new PuntuacionFecha { FutbolistaId = 7, NumeroFecha = 4, Puntaje = 8m };

            ActionResult resultado = _controlador.Cargar(puntuacion);

            OkObjectResult ok = Assert.IsType<OkObjectResult>(resultado);
            Assert.Contains("5", ok.Value!.ToString());
            _servicioFalso.Verify(s => s.Cargar(puntuacion), Times.Once);
        }

        [Fact]
        public void T2_Cargar_ReglaDeNegocioFalla_DevuelveBadRequestConElMensaje()
        {
            _servicioFalso.Setup(s => s.Cargar(It.IsAny<PuntuacionFecha>()))
                          .Throws(new ReglaNegocioException("La fecha tiene que estar entre 1 y 49"));

            ActionResult resultado = _controlador.Cargar(new PuntuacionFecha());

            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Contains("La fecha tiene que estar entre 1 y 49", badRequest.Value!.ToString());
        }

        [Fact]
        public void T3_ObtenerPorFecha_DevuelveOkConLaLista()
        {
            var lista = new List<PuntuacionFecha>
            {
                new PuntuacionFecha { Id = 1, FutbolistaId = 7, NumeroFecha = 4, Puntaje = 8m }
            };
            _servicioFalso.Setup(s => s.ObtenerPorFecha(4)).Returns(lista);

            ActionResult<IEnumerable<PuntuacionFecha>> resultado = _controlador.ObtenerPorFecha(4);

            OkObjectResult ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Same(lista, ok.Value);
        }

        [Fact]
        public void T4_ObtenerPorFutbolista_DevuelveOkConLaLista()
        {
            var lista = new List<PuntuacionFecha>
            {
                new PuntuacionFecha { Id = 2, FutbolistaId = 7, NumeroFecha = 4, Puntaje = 9m }
            };
            _servicioFalso.Setup(s => s.ObtenerPorFutbolista(7)).Returns(lista);

            ActionResult<IEnumerable<PuntuacionFecha>> resultado = _controlador.ObtenerPorFutbolista(7);

            OkObjectResult ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Same(lista, ok.Value);
        }
    }
}
