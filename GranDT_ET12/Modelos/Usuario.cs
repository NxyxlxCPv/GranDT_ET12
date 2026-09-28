namespace GranDT_ET12.Modelos
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Password { get; set; } = string.Empty; // Encriptada en 64 caracteres
        public bool EsAdministrador { get; set; }

    }
}