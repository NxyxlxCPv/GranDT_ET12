using System.Data;
using Dapper;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using MySqlConnector;

namespace GranDT_ET12.Repositorios
{
    public class PlantillaRepositorio : IPlantillaRepositorio
    {
        private readonly string _connectionString;

        public PlantillaRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CrearPlantilla(Plantilla plantilla)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.ExecuteScalar<int>("SP_RegistrarPlantilla", new
            {
                p_UsuarioId = plantilla.UsuarioId,
                p_PresupuestoMaximo = plantilla.PresupuestoMaximo,
                p_CantidadMaximaJugadores = plantilla.CantidadMaximaJugadores
            }, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<Plantilla> ObtenerPorUsuarioId(int usuarioId)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.Query<Plantilla>("SELECT * FROM Plantilla WHERE UsuarioId = @usuarioId", new { usuarioId }).ToList();
        }

        public decimal ObtenerGastoTotalPlantilla(int plantillaId)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.ExecuteScalar<decimal>("SELECT FN_CalcularGastoPlantilla(@plantillaId)", new { plantillaId });
        }

        public Plantilla? ObtenerPorId(int id)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.QueryFirstOrDefault<Plantilla>("SELECT * FROM Plantilla WHERE Id = @id", new { id });
        }
    }
}
