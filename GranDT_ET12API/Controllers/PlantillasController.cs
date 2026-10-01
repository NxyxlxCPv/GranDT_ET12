using GranDT_ET12.Modelos;
using GranDT_ET12.Service;
using GranDT_ET12.Service.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GranDT_ET12API.Controllers
{
    [ApiController]
    [Route("plantillas")]
    public class PlantillasController : ControllerBase
    {
        private IPlantillaService _plantillaService;

        public PlantillasController(IPlantillaService plantillaService)
        {
            _plantillaService = plantillaService;
        }

        // GET /plantillas/5
        [HttpGet("{id}")]
        public ActionResult<Plantilla> ObtenerPorId(int id)
        {
            Plantilla? plantilla = _plantillaService.ObtenerPorId(id);
            if (plantilla == null)
            {
                return NotFound(new { error = "La plantilla no existe." });
            }
            return Ok(plantilla);
        }

        // Muchos a muchos: los jugadores de una plantilla

        // GET /plantillas/5/jugadores
        [HttpGet("{plantillaId}/jugadores")]
        public ActionResult<IEnumerable<DetallePlantilla>> ObtenerJugadores(int plantillaId)
        {
            return Ok(_plantillaService.ObtenerDetalle(plantillaId));
        }

        // POST /plantillas/5/jugadores/7?esTitular=true
        [HttpPost("{plantillaId}/jugadores/{futbolistaId}")]
        public ActionResult AgregarJugador(int plantillaId, int futbolistaId, bool esTitular)
        {
            try
            {
                _plantillaService.AgregarJugador(plantillaId, futbolistaId, esTitular);
                return Ok(new { mensaje = "Jugador agregado a la plantilla." });
            }
            catch (ReglaNegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE /plantillas/5/jugadores/7
        [HttpDelete("{plantillaId}/jugadores/{futbolistaId}")]
        public ActionResult QuitarJugador(int plantillaId, int futbolistaId)
        {
            bool seElimino = _plantillaService.QuitarJugador(plantillaId, futbolistaId);
            if (!seElimino)
            {
                return NotFound(new { error = "El futbolista no esta en la plantilla." });
            }
            return Ok(new { mensaje = "Jugador quitado de la plantilla." });
        }
    }
}
