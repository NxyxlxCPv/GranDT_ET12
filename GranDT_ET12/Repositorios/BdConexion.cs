namespace GranDT_ET12.Repositorios
{
    public class BdConexion
    {
        private const string CadenaMySql
            = "Server=localhost;Port=3306;Database=GranDT_ET12;Uid=root;Pwd=Nayeli1423;";

        public static string ObtenerConnectionString()
            => CadenaMySql;
    }
}
