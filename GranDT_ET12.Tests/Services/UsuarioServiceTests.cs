using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using GranDT_ET12.Seguridad;
using GranDT_ET12.Service;
using Moq;
using Xunit;

namespace GranDT_ET12.Tests.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _usuarioService = new UsuarioService(_usuarioRepositorioMock.Object);
        }

        [Fact]
        public void T1_Registrar_DatosValidos_EncriptaPasswordYDevuelveId()
        {
            var usuario = new Usuario { Nombre = "Ana", Apellido = "Lopez", Email = "ana@mail.com" };
            _usuarioRepositorioMock.Setup(r => r.ObtenerPorEmail(usuario.Email)).Returns((Usuario?)null);
            _usuarioRepositorioMock.Setup(r => r.ObtenerTodos()).Returns(new List<Usuario>());
            _usuarioRepositorioMock.Setup(r => r.InsertarUsuario(It.IsAny<Usuario>())).Returns(11);

            int resultado = _usuarioService.Registrar(usuario, "1234");

            Assert.Equal(11, resultado);
            Assert.Equal(Encriptador.EncriptarPassword("1234"), usuario.Password);
            Assert.NotEqual("1234", usuario.Password);
            _usuarioRepositorioMock.Verify(r => r.InsertarUsuario(usuario), Times.Once);
        }

        [Fact]
        public void T2_Registrar_EmailDuplicado_LanzaReglaNegocioException()
        {
            var usuario = new Usuario { Nombre = "Ana", Apellido = "Lopez", Email = "ana@mail.com" };
            _usuarioRepositorioMock.Setup(r => r.ObtenerPorEmail(usuario.Email))
                                   .Returns(new Usuario { Id = 1, Email = usuario.Email });

            var excepcion = Assert.Throws<ReglaNegocioException>(() => _usuarioService.Registrar(usuario, "1234"));

            Assert.Equal("Ya existe un usuario con ese email.", excepcion.Message);
            _usuarioRepositorioMock.Verify(r => r.InsertarUsuario(It.IsAny<Usuario>()), Times.Never);
        }
    }
}
