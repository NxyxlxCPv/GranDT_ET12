using GranDT_ET12.Modelos;
using GranDT_ET12.Service;
using GranDT_ET12.Service.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GranDT_ET12API.Controllers
{
    [ApiController]
    [Route("futbolistas")]
    public class FutbolistasController : ControllerBase
    {
        private IFutbolistaService _futbolistaService;

        public FutbolistasController(IFutbolistaService futbolistaService)
        {
            _futbolistaService = futbolistaService;
        }

        // POST /futbolistas
        [HttpPost]
        public ActionResult Crear(Futbolista futbolista)
        {
            try
            {
                int id = _futbolistaService.Crear(futbolista);
                return Ok(new { id });
            }
            catch (ReglaNegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET /futbolistas
        // Se puede filtrar: /futbolistas?equipoId=1  o  /futbolistas?tipo=Arquero
        [HttpGet]
        public ActionResult<IEnumerable<Futbolista>> ObtenerTodos(int? equipoId, string? tipo)
        {
            if (equipoId != null)
            {
                return Ok(_futbolistaService.ObtenerPorEquipo(equipoId.Value));
            }
            if (tipo != null)
            {
                try
                {
                    return Ok(_futbolistaService.ObtenerPorPosicion(tipo));
                }
                catch (ReglaNegocioException ex)
                {
                    return BadRequest(new { error = ex.Message });
                }
            }
            return Ok(_futbolistaService.ObtenerTodos());
        }

        // GET /futbolistas/5
        [HttpGet("{id}")]
        public ActionResult<Futbolista> ObtenerPorId(int id)
        {
            Futbolista? futbolista = _futbolistaService.ObtenerPorId(id);
            if (futbolista == null)
            {
                return NotFound(new { error = "El futbolista no existe." });
            }
            return Ok(futbolista);
        }
    }
}
