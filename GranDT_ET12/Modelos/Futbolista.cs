
namespace GranDT_ET12.Modelos
{
    public class Futbolista
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Apodo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Tipo { get; set; } = string.Empty; //Arquero, Defensor, Mediocampista.....
        public decimal Cotizacion { get; set; }
        public int EquipoId { get; set; }
    }
}
