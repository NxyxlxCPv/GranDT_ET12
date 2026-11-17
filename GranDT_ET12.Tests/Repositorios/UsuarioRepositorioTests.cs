using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios;
using Xunit;

namespace GranDT_ET12.Tests.Repositorios
{
    public class UsuarioRepositorioTests
    {
        private readonly string _cadenaConexion = "Server=localhost;Database=GranDT_ET12;Uid=root;Pwd=Nayeli1423;";
        private readonly UsuarioRepositorio _repo;

        public UsuarioRepositorioTests()
        {
            _repo = new UsuarioRepositorio(_cadenaConexion);
        }

        [Fact]
        public void T1_ObtenerTodos_DevuelveUsuarios()
        {
            var resultado = _repo.ObtenerTodos();

            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado);
        }

        [Fact]
        public void T2_ObtenerPorId_UsuarioNoExiste_DevuelveNull()
        {
            var resultado = _repo.ObtenerPorId(-1);

            Assert.Null(resultado);
        }
    }
}
