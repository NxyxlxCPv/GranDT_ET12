using GranDT_ET12.Modelos;
using GranDT_ET12.Service;
using GranDT_ET12.Service.interfaces;
using GranDT_ET12API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GranDT_ET12.Tests.Controllers
{
    public class EquiposControllerTests
    {
        private Mock<IEquipoService> _servicioFalso;
        private EquiposController _controlador;

        public EquiposControllerTests()
        {
            _servicioFalso = new Mock<IEquipoService>();
            _controlador = new EquiposController(_servicioFalso.Object);
        }

        // ---------- Crear ----------

        [Fact]
        public void Crear_DatosValidos_DevuelveOkConElId()
        {
            _servicioFalso.Setup(s => s.Crear(It.IsAny<Equipo>())).Returns(4);
            Equipo equipo = new Equipo { Nombre = "Boca" };

            ActionResult resultado = _controlador.Crear(equipo);

            OkObjectResult ok = Assert.IsType<OkObjectResult>(resultado);
            Assert.Contains("4", ok.Value!.ToString());
        }

        [Fact]
        public void Crear_ReglaDeNegocioFalla_DevuelveBadRequestConElMensaje()
        {
            _servicioFalso.Setup(s => s.Crear(It.IsAny<Equipo>()))
                          .Throws(new ReglaNegocioException("Se alcanzo el maximo de equipos."));

            ActionResult resultado = _controlador.Crear(new Equipo());

            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Contains("Se alcanzo el maximo de equipos.", badRequest.Value!.ToString());
        }

        // ---------- Obtener ----------

        [Fact]
        public void ObtenerTodos_DevuelveOkConLaLista()
        {
            List<Equipo> lista = new List<Equipo> { new Equipo { Id = 1 }, new Equipo { Id = 2 } };
            _servicioFalso.Setup(s => s.ObtenerTodos()).Returns(lista);

            ActionResult<IEnumerable<Equipo>> resultado = _controlador.ObtenerTodos();

            OkObjectResult ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Same(lista, ok.Value);
        }

        [Fact]
        public void ObtenerPorId_EquipoExiste_DevuelveOk()
        {
            Equipo equipo = new Equipo { Id = 5, Nombre = "River" };
            _servicioFalso.Setup(s => s.ObtenerPorId(5)).Returns(equipo);

            ActionResult<Equipo> resultado = _controlador.ObtenerPorId(5);

            OkObjectResult ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Same(equipo, ok.Value);
        }

        [Fact]
        public void ObtenerPorId_EquipoNoExiste_DevuelveNotFound()
        {
            _servicioFalso.Setup(s => s.ObtenerPorId(99)).Returns((Equipo?)null);

            ActionResult<Equipo> resultado = _controlador.ObtenerPorId(99);

            Assert.IsType<NotFoundObjectResult>(resultado.Result);
        }
    }
}
