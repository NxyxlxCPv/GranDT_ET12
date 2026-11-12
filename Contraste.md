Actividad 1: Reglas, capas y pruebas pequeñas

Separación de capas: Tener definido el rol del Modelo, los Servicios, los Repositorios (con Dapper y MySQL) y la API.

Prueba de concepto (PoC): Incluye el código de una Minimal API con Program.cs, el uso de Swashbuckle.AspNetCore para Swagger y cómo se conecta con el proyecto real.   


Preguntas guía:

¿Cómo evitar repetir un futbolista en una plantilla?

En el servicio: Antes de guardar, consultamos la lista de la plantilla. Si el jugador ya está, lanzamos una excepción con un mensaje claro.

En MySQL: La tabla intermedia DetallePlantilla tiene una clave primaria compuesta o restricción UNIQUE (PlantillaId, FutbolistaId). La base jamás va a dejar insertar un duplicado.

¿Qué pasa si falta la puntuación en una fecha?

No inventamos un 0, porque cero no es lo mismo que "no jugó" o "no se cargó el dato". Usamos un LEFT JOIN entre Futbolista y PuntuacionFecha y manejamos el puntaje como nullable. Si no hay fila para esa fecha, la API devuelve null o no incluye esa fecha en el historial, distinguiendo la falta de dato de un cero real.

¿Qué diferencia hay entre validar en el servicio y usar permisos en MySQL?

Validación en el servicio: Revisa la lógica del negocio (rango de puntajes, presupuesto, límite de 20 jugadores) y devuelve mensajes claros al usuario final.
Permisos y restricciones en MySQL: Protege la integridad física de los datos mediante UNIQUE, CHECK y claves foráneas, además de limitar qué puede hacer la app en la base (privilegios mínimos)
