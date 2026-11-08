using System.Data;
using Dapper;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using MySqlConnector;

namespace GranDT_ET12.Repositorios
{
    public class DetallePlantillaRepositorio : IDetallePlantillaRepositorio
    {
        private readonly string _connectionString;

        public DetallePlantillaRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool AgregarJugador(DetallePlantilla detalle)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.Execute("SP_AgregarJugadorPlantilla", new
            {
                p_PlantillaId = detalle.PlantillaId,
                p_FutbolistaId = detalle.FutbolistaId,
                p_EsTitular = detalle.EsTitular
            }, commandType: CommandType.StoredProcedure) > 0;
        }

        public bool EliminarJugador(int plantillaId, int futbolistaId)
        {
            using var conexion = new MySqlConnection(_connectionString);
            const string sql = "DELETE FROM DetallePlantilla WHERE PlantillaId = @plantillaId AND FutbolistaId = @futbolistaId";
            return conexion.Execute(sql, new { plantillaId, futbolistaId }) > 0;
        }

        public IEnumerable<DetallePlantilla> ObtenerPorPlantillaId(int plantillaId)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.Query<DetallePlantilla>("SELECT * FROM DetallePlantilla WHERE PlantillaId = @plantillaId", new { plantillaId }).ToList();
        }

        public int ContarJugadoresEnPlantilla(int plantillaId)
        {
            using var conexion = new MySqlConnection(_connectionString);
            return conexion.ExecuteScalar<int>("SELECT COUNT(*) FROM DetallePlantilla WHERE PlantillaId = @plantillaId", new { plantillaId });
        }
    }
}
