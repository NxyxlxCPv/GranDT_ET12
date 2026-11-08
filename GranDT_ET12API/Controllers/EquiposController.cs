using GranDT_ET12.Modelos;
using GranDT_ET12.Service;
using GranDT_ET12.Service.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GranDT_ET12API.Controllers
{
    [ApiController]
    [Route("equipos")]
    public class EquiposController : ControllerBase
    {
        private IEquipoService _equipoService;

        public EquiposController(IEquipoService equipoService)
        {
            _equipoService = equipoService;
        }

        // POST /equipos
        [HttpPost]
        public ActionResult Crear(Equipo equipo)
        {
            try
            {
                int id = _equipoService.Crear(equipo);
                return Ok(new { id });
            }
            catch (ReglaNegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // GET /equipos
        [HttpGet]
        public ActionResult<IEnumerable<Equipo>> ObtenerTodos()
        {
            return Ok(_equipoService.ObtenerTodos());
        }

        // GET /equipos/5
        [HttpGet("{id}")]
        public ActionResult<Equipo> ObtenerPorId(int id)
        {
            Equipo? equipo = _equipoService.ObtenerPorId(id);
            if (equipo == null)
            {
                return NotFound(new { error = "El equipo no existe." });
            }
            return Ok(equipo);
        }
    }
}
