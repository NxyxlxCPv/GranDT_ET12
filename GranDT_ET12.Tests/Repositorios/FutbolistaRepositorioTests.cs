using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios;
using Xunit;

namespace GranDT_ET12.Tests.Repositorios
{
    public class FutbolistaRepositorioTests
    {
        private readonly string _cadenaConexion = "Server=localhost;Database=GranDT_ET12;Uid=root;Pwd=Nayeli1423;";
        private readonly FutbolistaRepositorio _repo;

        public FutbolistaRepositorioTests()
        {
            _repo = new FutbolistaRepositorio(_cadenaConexion);
        }

        [Fact]
        public void ObtenerTodos_DevuelveFutbolistas()
        {
            var resultado = _repo.ObtenerTodos();

            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado);
        }

        [Fact]
        public void ObtenerPorId_FutbolistaExiste_DevuelveFutbolista()
        {
            var resultado = _repo.ObtenerPorId(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public void ObtenerPorId_FutbolistaNoExiste_DevuelveNull()
        {
            var resultado = _repo.ObtenerPorId(-1);

            Assert.Null(resultado);
        }

        [Fact]
        public void ObtenerPorEquipo_DevuelveFutbolistas()
        {
            var resultado = _repo.ObtenerPorEquipo(1);

            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado);
        }

        [Fact]
        public void ObtenerPorPosicion_DevuelveFutbolistas()
        {
            var resultado = _repo.ObtenerPorPosicion("Arquero");

            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado);
        }
    }
}
