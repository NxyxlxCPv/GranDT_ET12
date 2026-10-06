using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios;
using Xunit;

namespace GranDT_ET12.Tests.Repositorios
{
    public class PlantillaRepositorioTests
    {
        private readonly string _cadenaConexion = "Server=localhost;Database=GranDT_ET12;Uid=root;Pwd=Nayeli1423;";
        private readonly PlantillaRepositorio _repo;

        public PlantillaRepositorioTests()
        {
            _repo = new PlantillaRepositorio(_cadenaConexion);
        }

        [Fact]
        public void CrearPlantilla_PlantillaValida_DevuelveIdGenerado()
        {
            var plantilla = new Plantilla
            {
                UsuarioId = 1,
                PresupuestoMaximo = 500000000m,
                CantidadMaximaJugadores = 20
            };

            int nuevoId = _repo.CrearPlantilla(plantilla);

            Assert.True(nuevoId > 0);
        }

        [Fact]
        public void ObtenerPorId_PlantillaExiste_DevuelvePlantilla()
        {
            var plantilla = new Plantilla
            {
                UsuarioId = 1,
                PresupuestoMaximo = 500000000m,
                CantidadMaximaJugadores = 20
            };
            int nuevoId = _repo.CrearPlantilla(plantilla);

            var resultado = _repo.ObtenerPorId(nuevoId);

            Assert.NotNull(resultado);
            Assert.Equal(nuevoId, resultado.Id);
            Assert.Equal(1, resultado.UsuarioId);
        }

        [Fact]
        public void ObtenerPorId_PlantillaNoExiste_DevuelveNull()
        {
            var resultado = _repo.ObtenerPorId(-1);

            Assert.Null(resultado);
        }

        [Fact]
        public void ObtenerPorUsuarioId_DevuelvePlantillasDelUsuario()
        {
            var resultado = _repo.ObtenerPorUsuarioId(1);

            Assert.NotNull(resultado);
        }
    }
}
