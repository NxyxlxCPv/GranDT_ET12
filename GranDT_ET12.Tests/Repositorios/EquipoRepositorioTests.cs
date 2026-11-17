using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios;
using Xunit;

namespace GranDT_ET12.Tests.Repositorios
{
    public class EquipoRepositorioTests
    {
        private readonly string _cadenaConexion = "Server=localhost;Database=GranDT_ET12;Uid=root;Pwd=Nayeli1423;";
        private readonly EquipoRepositorio _repo;

        public EquipoRepositorioTests()
        {
            _repo = new EquipoRepositorio(_cadenaConexion);
        }

        [Fact]
        public void T1_Test1_ObtenerTodos_DevuelveEquipos()
        {
            var resultado = _repo.ObtenerTodos();

            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado);
        }

        [Fact]
        public void T2_ObtenerPorId_EquipoExiste_DevuelveEquipo()
        {
            var resultado = _repo.ObtenerPorId(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public void T3_ObtenerPorId_EquipoNoExiste_DevuelveNull()
        {
            var resultado = _repo.ObtenerPorId(-1);

            Assert.Null(resultado);
        }
    }
}
