namespace GranDT_ET12.Modelos
{
    // Datos para registrar un usuario. No tiene EsAdministrador: siempre se crea un usuario comun.
    public class UsuarioRegistro
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
