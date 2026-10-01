using Microsoft.AspNetCore.Mvc;

namespace GranDT_ET12API.Controllers
{
    // Cuando ocurre un error que no controlamos (por ejemplo, la base de datos no responde),
    // ASP.NET manda la peticion a esta ruta (configurado en Program.cs con UseExceptionHandler).
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)] // para que esta ruta no aparezca en Swagger
    [Route("error")]
    public class ErrorController : ControllerBase
    {
        public ActionResult Error()
        {
            return StatusCode(500, new { error = "Ocurrio un error inesperado en el servidor." });
        }
    }
}
