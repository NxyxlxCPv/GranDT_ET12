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
            const string sql = @"INSERT INTO Plantilla (UsuarioId, PresupuestoMaximo, CantidadMaximaJugadores)
                                 VALUES (@UsuarioId, @PresupuestoMaximo, @CantidadMaximaJugadores);
                                 SELECT LAST_INSERT_ID();";
            return conexion.ExecuteScalar<int>(sql, plantilla);
        }

        public IEnumerable<Plantilla> ObtenerPorUsuarioId(int usuarioId)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.Query<Plantilla>("SELECT * FROM Plantilla WHERE UsuarioId = @usuarioId", new { usuarioId }).ToList();
        }

        public Plantilla? ObtenerPorId(int id)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.QueryFirstOrDefault<Plantilla>("SELECT * FROM Plantilla WHERE Id = @id", new { id });
        }
    }
}
