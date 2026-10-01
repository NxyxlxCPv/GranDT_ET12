using Dapper;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using MySqlConnector;

namespace GranDT_ET12.Repositorios
{
    public class PuntuacionRepositorio : IPuntuacionRepositorio
    {
        private readonly string _connectionString;

        public PuntuacionRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int InsertarPuntuacion(PuntuacionFecha puntuacion)
        {
            using var conexion = new MySqlConnection(_connectionString);
            const string sql = @"INSERT INTO PuntuacionFecha (FutbolistaId, NumeroFecha, Puntaje)
                                 VALUES (@FutbolistaId, @NumeroFecha, @Puntaje);
                                 SELECT LAST_INSERT_ID();";
            return conexion.ExecuteScalar<int>(sql, puntuacion);
        }

        public bool ExistePuntuacionEnFecha(int futbolistaId, int numeroFecha)
        {
            using var conexion = new MySqlConnection(_connectionString);
            const string sql = "SELECT COUNT(*) FROM PuntuacionFecha WHERE FutbolistaId = @futbolistaId AND NumeroFecha = @numeroFecha";
            return conexion.ExecuteScalar<int>(sql, new { futbolistaId, numeroFecha }) > 0;
        }

        public IEnumerable<PuntuacionFecha> ObtenerPuntuacionesPorFutbolista(int futbolistaId)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.Query<PuntuacionFecha>("SELECT * FROM PuntuacionFecha WHERE FutbolistaId = @futbolistaId", new { futbolistaId }).ToList();
        }

        public IEnumerable<PuntuacionFecha> ObtenerPuntuacionesPorFecha(int numeroFecha)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.Query<PuntuacionFecha>("SELECT * FROM PuntuacionFecha WHERE NumeroFecha = @numeroFecha", new { numeroFecha }).ToList();
        }
    }
}
