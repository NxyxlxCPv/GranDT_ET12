namespace GranDT_ET12.Configuracion
{
    // Valores fijos del sistema (limites del enunciado). Si algo cambia, se cambia aca.
    public static class Constantes
    {
        // Limites de cantidad
        public const int MaxEquipos = 32;
        public const int MaxFutbolistas = 1500;
        public const int MaxUsuarios = 2000;
        public const int MaxJugadoresPlantilla = 20;

        // Presupuesto igual para todas las plantillas (valor a confirmar con el profe)
        public const decimal PresupuestoMaximoPlantilla = 500_000_000.00m;

        // Futbolista
        public const decimal CotizacionMaxima = 99_999_999.99m;

        // Puntuaciones
        public const int FechaMinima = 1;
        public const int FechaMaxima = 49; // el numero de fecha tiene que ser menor a 50
        public const decimal PuntajeMinimo = 1m;
        public const decimal PuntajeMaximo = 10m;

        // Contraseña encriptada (SHA-256 en hexadecimal)
        public const int LongitudPassword = 64;

        // Titulares de una plantilla valida (pendiente de usar en Plantilla)
        public const int TitularesArquero = 1;
        public const int TitularesDefensor = 4;
        public const int TitularesMediocampista = 4;
        public const int TitularesDelantero = 2;
    }
}
