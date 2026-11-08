using System.Data;
using Dapper;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using MySqlConnector;

namespace GranDT_ET12.Repositorios;

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
        return conexion.ExecuteScalar<int>("SP_RegistrarPuntuacion", new
        {
            p_FutbolistaId = puntuacion.FutbolistaId,
            p_NumeroFecha = puntuacion.NumeroFecha,
            p_Puntaje = puntuacion.Puntaje
        }, commandType: CommandType.StoredProcedure);
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
        return conexion.Query<PuntuacionFecha>(
            "SELECT * FROM PuntuacionFecha WHERE FutbolistaId = @futbolistaId",
            new { futbolistaId });
    }

    public IEnumerable<PuntuacionFecha> ObtenerPuntuacionesPorFecha(int numeroFecha)
    {
        using var conexion = new MySqlConnection(_connectionString);
        return conexion.Query<PuntuacionFecha>("SELECT * FROM PuntuacionFecha WHERE NumeroFecha = @numeroFecha", new { numeroFecha }).ToList();
    }

    public decimal ObtenerPuntuacionPlantillaFecha(int plantillaId, int numeroFecha)
    {
        using var conexion = new MySqlConnection(_connectionString);
        return conexion.ExecuteScalar<decimal>("SELECT FN_CalcularPuntuacionPlantillaFecha(@plantillaId, @numeroFecha)", new { plantillaId, numeroFecha });
    }

}
