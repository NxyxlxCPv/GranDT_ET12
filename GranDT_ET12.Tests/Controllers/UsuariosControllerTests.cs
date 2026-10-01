using GranDT_ET12.Modelos;
using GranDT_ET12.Service;
using GranDT_ET12.Service.interfaces;
using GranDT_ET12API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GranDT_ET12.Tests.Controllers
{
    public class UsuariosControllerTests
    {
        private Mock<IUsuarioService> _usuarioServicioFalso;
        private Mock<IPlantillaService> _plantillaServicioFalso;
        private UsuariosController _controlador;

        public UsuariosControllerTests()
        {
            _usuarioServicioFalso = new Mock<IUsuarioService>();
            _plantillaServicioFalso = new Mock<IPlantillaService>();
            _controlador = new UsuariosController(_usuarioServicioFalso.Object, _plantillaServicioFalso.Object);
        }

        private UsuarioRegistro CrearPeticion()
        {
            return new UsuarioRegistro
            {
                Nombre = "Ana",
                Apellido = "Lopez",
                Email = "ana@mail.com",
                FechaNacimiento = new DateOnly(2000, 5, 20),
                Password = "1234"
            };
        }

        // ---------- Registrar ----------

        [Fact]
        public void Registrar_DatosValidos_DevuelveOkConElId()
        {
            _usuarioServicioFalso.Setup(s => s.Registrar(It.IsAny<Usuario>(), "1234")).Returns(7);

            ActionResult resultado = _controlador.Registrar(CrearPeticion());

            OkObjectResult ok = Assert.IsType<OkObjectResult>(resultado);
            Assert.Contains("7", ok.Value!.ToString());
        }

        [Fact]
        public void Registrar_SiempreCreaUsuarioComun_NuncaAdministrador()
        {
            Usuario? usuarioEnviado = null;
            _usuarioServicioFalso.Setup(s => s.Registrar(It.IsAny<Usuario>(), It.IsAny<string>()))
                                 .Callback<Usuario, string>((u, p) => usuarioEnviado = u)
                                 .Returns(1);

            _controlador.Registrar(CrearPeticion());

            Assert.NotNull(usuarioEnviado);
            Assert.False(usuarioEnviado!.EsAdministrador);
            Assert.Equal("Ana", usuarioEnviado.Nombre);
            Assert.Equal("ana@mail.com", usuarioEnviado.Email);
        }

        [Fact]
        public void Registrar_EmailDuplicado_DevuelveBadRequest()
        {
            _usuarioServicioFalso.Setup(s => s.Registrar(It.IsAny<Usuario>(), It.IsAny<string>()))
                                 .Throws(new ReglaNegocioException("El email ya esta registrado."));

            ActionResult resultado = _controlador.Registrar(CrearPeticion());

            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Contains("El email ya esta registrado.", badRequest.Value!.ToString());
        }

        // ---------- Obtener ----------

        [Fact]
        public void ObtenerTodos_DevuelveOkConLaLista()
        {
            List<Usuario> lista = new List<Usuario> { new Usuario { Id = 1 }, new Usuario { Id = 2 } };
            _usuarioServicioFalso.Setup(s => s.ObtenerTodos()).Returns(lista);

            ActionResult<IEnumerable<Usuario>> resultado = _controlador.ObtenerTodos();

            OkObjectResult ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Same(lista, ok.Value);
        }

        [Fact]
        public void ObtenerPorId_UsuarioExiste_DevuelveOk()
        {
            Usuario usuario = new Usuario { Id = 5 };
            _usuarioServicioFalso.Setup(s => s.ObtenerPorId(5)).Returns(usuario);

            ActionResult<Usuario> resultado = _controlador.ObtenerPorId(5);

            OkObjectResult ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Same(usuario, ok.Value);
        }

        [Fact]
        public void ObtenerPorId_UsuarioNoExiste_DevuelveNotFound()
        {
            _usuarioServicioFalso.Setup(s => s.ObtenerPorId(99)).Returns((Usuario?)null);

            ActionResult<Usuario> resultado = _controlador.ObtenerPorId(99);

            Assert.IsType<NotFoundObjectResult>(resultado.Result);
        }

        // ---------- Plantillas del usuario ----------

        [Fact]
        public void CrearPlantilla_Correcto_DevuelveOkConElId()
        {
            _plantillaServicioFalso.Setup(s => s.Crear(3)).Returns(10);

            ActionResult resultado = _controlador.CrearPlantilla(3);

            OkObjectResult ok = Assert.IsType<OkObjectResult>(resultado);
            Assert.Contains("10", ok.Value!.ToString());
        }

        [Fact]
        public void CrearPlantilla_ReglaDeNegocioFalla_DevuelveBadRequest()
        {
            _plantillaServicioFalso.Setup(s => s.Crear(3)).Throws(new ReglaNegocioException("El usuario no existe."));

            ActionResult resultado = _controlador.CrearPlantilla(3);

            Assert.IsType<BadRequestObjectResult>(resultado);
        }

        [Fact]
        public void ObtenerPlantillas_DevuelveOkConLasPlantillasDelUsuario()
        {
            List<Plantilla> plantillas = new List<Plantilla> { new Plantilla() };
            _plantillaServicioFalso.Setup(s => s.ObtenerPorUsuario(3)).Returns(plantillas);

            ActionResult<IEnumerable<Plantilla>> resultado = _controlador.ObtenerPlantillas(3);

            OkObjectResult ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Same(plantillas, ok.Value);
        }
    }
}
