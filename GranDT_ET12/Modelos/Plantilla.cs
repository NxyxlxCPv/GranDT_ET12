
namespace GranDT_ET12.Modelos
{
    public class Plantilla
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public decimal PresupuestoMaximo { get; set; }
        public int CantidadMaximaJugadores { get; set; } = 20;
    }
}
