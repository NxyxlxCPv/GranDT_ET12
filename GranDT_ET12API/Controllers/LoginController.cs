using GranDT_ET12.Modelos;
using GranDT_ET12.Service;
using Microsoft.AspNetCore.Mvc;

namespace GranDT_ET12API.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private LoginService _loginService;

        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }

        // POST /login
        [HttpPost]
        public ActionResult<Usuario> IniciarSesion(Login peticion)
        {
            try
            {
                Usuario? usuario = _loginService.Login(peticion.Email, peticion.Password);
                if (usuario == null)
                {
                    return Unauthorized(new { error = "Email o contraseña incorrectos." });
                }
                return Ok(usuario);
            }
            catch (ReglaNegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
