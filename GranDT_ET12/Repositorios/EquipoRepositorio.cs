using Dapper;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using MySqlConnector;

namespace GranDT_ET12.Repositorios
{
    public class EquipoRepositorio : IEquipoRepositorio
    {
        private readonly string _connectionString;

        public EquipoRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int InsertarEquipo(Equipo equipo)
        {
            using var conexion = new MySqlConnection(_connectionString);
            const string sql = @"INSERT INTO Equipo (Nombre) VALUES (@Nombre);
                                 SELECT LAST_INSERT_ID();";
            return conexion.ExecuteScalar<int>(sql, equipo);
        }

        public Equipo? ObtenerPorId(int id)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.QueryFirstOrDefault<Equipo>("SELECT * FROM Equipo WHERE Id = @id", new { id });
        }

        public IEnumerable<Equipo> ObtenerTodos()
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.Query<Equipo>("SELECT * FROM Equipo").ToList();
        }
    }
}
