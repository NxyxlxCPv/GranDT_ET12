using GranDT_ET12.Modelos;
using GranDT_ET12.Service;
using GranDT_ET12.Service.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GranDT_ET12API.Controllers
{
    [ApiController]
    [Route("puntuaciones")]
    public class PuntuacionesController : ControllerBase
    {
        private IPuntuacionService _puntuacionService;

        public PuntuacionesController(IPuntuacionService puntuacionService)
        {
            _puntuacionService = puntuacionService;
        }

        // POST /puntuaciones
        [HttpPost]
        public ActionResult Cargar(PuntuacionFecha puntuacion)
        {
            try
            {
                int id = _puntuacionService.Cargar(puntuacion);
                return Ok(new { id });
            }
            catch (ReglaNegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET /puntuaciones/fecha/4
        [HttpGet("fecha/{numeroFecha}")]
        public ActionResult<IEnumerable<PuntuacionFecha>> ObtenerPorFecha(int numeroFecha)
        {
            return Ok(_puntuacionService.ObtenerPorFecha(numeroFecha));
        }

        // GET /puntuaciones/futbolista/7
        [HttpGet("futbolista/{futbolistaId}")]
        public ActionResult<IEnumerable<PuntuacionFecha>> ObtenerPorFutbolista(int futbolistaId)
        {
            return Ok(_puntuacionService.ObtenerPorFutbolista(futbolistaId));
        }
    }
}
