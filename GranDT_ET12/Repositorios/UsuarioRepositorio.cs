using System.Data;
using Dapper;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using MySqlConnector;

namespace GranDT_ET12.Repositorios
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly string _connectionString;

        public UsuarioRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int InsertarUsuario(Usuario usuario)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.ExecuteScalar<int>("SP_RegistrarUsuario", new
            {
                p_Nombre = usuario.Nombre,
                p_Apellido = usuario.Apellido,
                p_Email = usuario.Email,
                p_FechaNacimiento = usuario.FechaNacimiento,
                p_Password = usuario.Password,
                p_EsAdministrador = usuario.EsAdministrador
            }, commandType: CommandType.StoredProcedure);
        }

        public Usuario? ObtenerPorId(int id)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.QueryFirstOrDefault<Usuario>("SELECT * FROM Usuario WHERE Id = @id", new { id });
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.QueryFirstOrDefault<Usuario>("SELECT * FROM Usuario WHERE Email = @email", new { email });
        }

        public Usuario? ObtenerPorCredenciales(string email, string passwordEncriptada)
        {
            using var conexion = new MySqlConnection(_connectionString);
            const string sql = "SELECT * FROM Usuario WHERE Email = @email AND Password = @passwordEncriptada";
            return conexion.QueryFirstOrDefault<Usuario>(sql, new { email, passwordEncriptada });
        }

        public IEnumerable<Usuario> ObtenerTodos()
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.Query<Usuario>("SELECT * FROM Usuario").ToList();
        }
    }
}
