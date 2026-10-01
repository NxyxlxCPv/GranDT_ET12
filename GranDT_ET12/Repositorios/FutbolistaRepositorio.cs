using Dapper;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using MySqlConnector;

namespace GranDT_ET12.Repositorios
{
    public class FutbolistaRepositorio : IFutbolistaRepositorio
    {
        private readonly string _connectionString;

        public FutbolistaRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int InsertarFutbolista(Futbolista futbolista)
        {
            using var conexion = new MySqlConnection(_connectionString);
            const string sql = @"INSERT INTO Futbolista (Nombre, Apellido, Apodo, FechaNacimiento, Tipo, Cotizacion, EquipoId)
                                 VALUES (@Nombre, @Apellido, @Apodo, @FechaNacimiento, @Tipo, @Cotizacion, @EquipoId);
                                 SELECT LAST_INSERT_ID();";
            // El Tipo se manda como texto porque en la BD es un ENUM de MySQL
            return conexion.ExecuteScalar<int>(sql, new
            {
                Nombre = futbolista.Nombre,
                Apellido = futbolista.Apellido,
                Apodo = futbolista.Apodo,
                FechaNacimiento = futbolista.FechaNacimiento,
                Tipo = futbolista.Tipo.ToString(),
                Cotizacion = futbolista.Cotizacion,
                EquipoId = futbolista.EquipoId
            });
        }

        public Futbolista? ObtenerPorId(int id)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.QueryFirstOrDefault<Futbolista>("SELECT * FROM Futbolista WHERE Id = @id", new { id });
        }

        public IEnumerable<Futbolista> ObtenerTodos()
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.Query<Futbolista>("SELECT * FROM Futbolista").ToList();
        }

        public IEnumerable<Futbolista> ObtenerPorEquipo(int equipoId)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.Query<Futbolista>("SELECT * FROM Futbolista WHERE EquipoId = @equipoId", new { equipoId }).ToList();
        }

        public IEnumerable<Futbolista> ObtenerPorPosicion(string tipo)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.Query<Futbolista>("SELECT * FROM Futbolista WHERE Tipo = @tipo", new { tipo }).ToList();
        }
    }
}
