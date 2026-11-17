using GranDT_ET12.Repositorios;
using Xunit;

namespace GranDT_ET12.Tests.Repositorios
{
    public class PuntuacionRepositorioTests
    {
        private readonly string _cadenaConexion = "Server=localhost;Database=GranDT_ET12;Uid=root;Pwd=Nayeli1423;";
        private readonly PuntuacionRepositorio _repo;

        public PuntuacionRepositorioTests()
        {
            _repo = new PuntuacionRepositorio(_cadenaConexion);
        }

        [Fact]
        public void T1_ExistePuntuacionEnFecha_FutbolistaSinPuntuacion_DevuelveFalse()
        {
            var resultado = _repo.ExistePuntuacionEnFecha(-1, 1);

            Assert.False(resultado);
        }

        [Fact]
        public void T2_ObtenerPuntuacionesPorFecha_FechaSinPuntuaciones_DevuelveVacio()
        {
            var resultado = _repo.ObtenerPuntuacionesPorFecha(-1);

            Assert.Empty(resultado);
        }
    }
}
