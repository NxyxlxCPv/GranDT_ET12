using GranDT_ET12.Repositorios;
using Xunit;

namespace GranDT_ET12.Tests.Repositorios
{
    public class DetallePlantillaRepositorioTests
    {
        private readonly string _cadenaConexion = "Server=localhost;Database=GranDT_ET12;Uid=root;Pwd=Nayeli1423;";
        private readonly DetallePlantillaRepositorio _repo;

        public DetallePlantillaRepositorioTests()
        {
            _repo = new DetallePlantillaRepositorio(_cadenaConexion);
        }

        [Fact]
        public void T1_ObtenerPorPlantillaId_PlantillaInexistente_DevuelveVacio()
        {
            var resultado = _repo.ObtenerPorPlantillaId(-1);

            Assert.Empty(resultado);
        }

        [Fact]
        public void T2_ContarJugadoresEnPlantilla_PlantillaInexistente_DevuelveCero()
        {
            var resultado = _repo.ContarJugadoresEnPlantilla(-1);

            Assert.Equal(0, resultado);
        }
    }
}
