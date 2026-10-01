
namespace GranDT_ET12.Modelos
{
    public class Futbolista
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Apodo { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public TipoFutbolista Tipo { get; set; } //Arquero, Defensor, Mediocampista, Delantero
        public decimal Cotizacion { get; set; }
        public int EquipoId { get; set; }
    }
}
