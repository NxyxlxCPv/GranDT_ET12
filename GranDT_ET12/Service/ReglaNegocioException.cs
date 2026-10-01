namespace GranDT_ET12.Service
{
    // Se lanza cuando no se cumple una regla del negocio (dato invalido, limite superado, duplicado...)
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje)
        {
        }
    }
}
