using System.Text.Json.Serialization;

namespace GranDT_ET12.Modelos
{
    [JsonConverter(typeof(JsonStringEnumConverter))] // en la API se ve "Arquero" y no 0
    public enum TipoFutbolista
    {
        Arquero,
        Defensor,
        Mediocampista,
        Delantero
    }
}
