using System.Security.Cryptography;
using System.Text;

namespace GranDT_ET12.Seguridad
{
    public static class Encriptador
    {
        // Devuelve el SHA-256 del texto en hexadecimal (siempre 64 caracteres)
        public static string EncriptarPassword(string texto)
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(texto));
            string resultado = Convert.ToHexString(hash);
            return resultado;
        }
    }
}
