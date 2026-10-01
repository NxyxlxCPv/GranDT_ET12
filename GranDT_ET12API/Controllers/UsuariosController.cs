using GranDT_ET12.Modelos;
using GranDT_ET12.Service;
using GranDT_ET12.Service.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GranDT_ET12API.Controllers
{
    [ApiController]
    [Route("usuarios")]
    public class UsuariosController : ControllerBase
    {
        private IUsuarioService _usuarioService;
        private IPlantillaService _plantillaService;

        public UsuariosController(IUsuarioService usuarioService, IPlantillaService plantillaService)
        {
            _usuarioService = usuarioService;
            _plantillaService = plantillaService;
        }

        // POST /usuarios  (registro: siempre crea un usuario comun, nunca un administrador)
        [HttpPost]
        public ActionResult Registrar(UsuarioRegistro peticion)
        {
            Usuario usuario = new Usuario();
            usuario.Nombre = peticion.Nombre;
            usuario.Apellido = peticion.Apellido;
            usuario.Email = peticion.Email;
            usuario.FechaNacimiento = peticion.FechaNacimiento;
            usuario.EsAdministrador = false;

            try
            {
                int id = _usuarioService.Registrar(usuario, peticion.Password);
                return Ok(new { id });
            }
            catch (ReglaNegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET /usuarios
        [HttpGet]
        public ActionResult<IEnumerable<Usuario>> ObtenerTodos()
        {
            return Ok(_usuarioService.ObtenerTodos());
        }

        // GET /usuarios/5
        [HttpGet("{id}")]
        public ActionResult<Usuario> ObtenerPorId(int id)
        {
            Usuario? usuario = _usuarioService.ObtenerPorId(id);
            if (usuario == null)
            {
                return NotFound(new { error = "El usuario no existe." });
            }
            return Ok(usuario);
        }

        // Un usuario puede tener varias plantillas

        // POST /usuarios/5/plantillas
        [HttpPost("{usuarioId}/plantillas")]
        public ActionResult CrearPlantilla(int usuarioId)
        {
            try
            {
                int id = _plantillaService.Crear(usuarioId);
                return Ok(new { id });
            }
            catch (ReglaNegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET /usuarios/5/plantillas
        [HttpGet("{usuarioId}/plantillas")]
        public ActionResult<IEnumerable<Plantilla>> ObtenerPlantillas(int usuarioId)
        {
            return Ok(_plantillaService.ObtenerPorUsuario(usuarioId));
        }
    }
}
