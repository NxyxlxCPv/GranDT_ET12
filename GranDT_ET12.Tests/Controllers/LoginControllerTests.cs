using GranDT_ET12.Modelos;
using GranDT_ET12.Service;
using GranDT_ET12.Service.interfaces;
using GranDT_ET12API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GranDT_ET12.Tests.Controllers
{
    public class LoginControllerTests
    {
        // Servicio falso: nos deja decidir que devuelve sin usar la base de datos
        private Mock<ILoginService> _servicioFalso;
        private LoginController _controlador;

        public LoginControllerTests()
        {
            _servicioFalso = new Mock<ILoginService>();
            _controlador = new LoginController(_servicioFalso.Object);
        }

        [Fact]
        public void IniciarSesion_CredencialesCorrectas_DevuelveOkConElUsuario()
        {
            Usuario usuario = new Usuario { Id = 1, Email = "ana@mail.com" };
            _servicioFalso.Setup(s => s.Login("ana@mail.com", "1234")).Returns(usuario);
            Login peticion = new Login { Email = "ana@mail.com", Password = "1234" };

            ActionResult<Usuario> resultado = _controlador.IniciarSesion(peticion);

            OkObjectResult ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Same(usuario, ok.Value);
        }

        [Fact]
        public void IniciarSesion_CredencialesIncorrectas_DevuelveUnauthorized()
        {
            _servicioFalso.Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>())).Returns((Usuario?)null);
            Login peticion = new Login { Email = "ana@mail.com", Password = "mala" };

            ActionResult<Usuario> resultado = _controlador.IniciarSesion(peticion);

            Assert.IsType<UnauthorizedObjectResult>(resultado.Result);
        }

        [Fact]
        public void IniciarSesion_ReglaDeNegocioFalla_DevuelveBadRequestConElMensaje()
        {
            _servicioFalso.Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>()))
                          .Throws(new ReglaNegocioException("El email es obligatorio."));
            Login peticion = new Login();

            ActionResult<Usuario> resultado = _controlador.IniciarSesion(peticion);

            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.Contains("El email es obligatorio.", badRequest.Value!.ToString());
        }
    }
}
