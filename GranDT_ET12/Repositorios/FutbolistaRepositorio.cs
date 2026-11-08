using System.Data;
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
            // El Tipo se manda como texto porque en la BD es un ENUM de MySQL
            return conexion.ExecuteScalar<int>("SP_RegistrarFutbolista", new
            {
                p_Nombre = futbolista.Nombre,
                p_Apellido = futbolista.Apellido,
                p_Apodo = futbolista.Apodo,
                p_FechaNacimiento = futbolista.FechaNacimiento,
                p_Tipo = futbolista.Tipo.ToString(),
                p_Cotizacion = futbolista.Cotizacion,
                p_EquipoId = futbolista.EquipoId
            }, commandType: CommandType.StoredProcedure);
        }

        public int ContarFutbolistas()
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.ExecuteScalar<int>("SELECT FN_ContarFutbolistas()");
        }

        public bool ExisteFutbolista(int id)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.ExecuteScalar<bool>("SELECT FN_ExisteFutbolista(@id)", new { id });
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
