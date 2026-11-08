RESPUESTA :

Objetos que intervienen
- Usuario: Propietario de la plantilla en el sistema.
- Plantilla: Contenedor de los futbolistas elegidos por el usuario.
- Futbolista: El jugador con su cotización, puesto/tipo y equipo.
- DetallePlantilla: Entidad intermedia que relaciona la plantilla con cada futbolista e indica si es titular o suplente.
- PuntuacionFecha: Representa la calificación asignada a un futbolista en una fecha específica del torneo. 
 

QUE PODRIA SALIR MAL :

- Que el valor total de los futbolistas incorporados exceda el límite de $100.000.000.
- Intentar agregar más de 20 futbolistas a la plantilla.
- Intentar cargar dos veces al mismo futbolista en la misma plantilla.
- No respetar la distribucion de titulares requerida (1 Arquero, 4 Defensores, etc)
- Intentar registrar una nota menor a 1, mayor a 10, o una fecha fuera del rango (1 a 49).
- Asignar más de una nota al mismo futbolista para la misma fecha.


HIPOTESIS SOBRE DONDE VALIDAR : 

- Hipótesis 1 : Las reglas de un solo dato se validan en el servicio antes de guardar. 
Cotización dentro de rango, puntaje entre 1 y 10, fecha entre 1 y 49, nombre obligatorio y 
email obligatorio no necesitan consultar a otros objetos. El servicio los rechaza apenas llegan, 
antes de tocar la base de datos.

- Hipótesis 2: Las reglas que dependen de varios objetos se validan en el servicio de plantillas. 
Presupuesto, máximo de 20 jugadores, futbolista repetido y composición de titulares necesitan 
información de la plantilla, de sus detalles y de cada futbolista (cotización y tipo). Ningún objeto 
aislado tiene todos esos datos.Por eso el servicio de plantillas los reúne, valida y recién después 
manda a guardar.

- Hipótesis 3: El endpoint no decide reglas: solo traduce entre HTTP y el servicio. El endpoint recibe 
los datos, llama al servicio y convierte el resultado en un código de estado (400 si los datos son inválidos, 
404 si no existe, 409 si está duplicado). Si las reglas estuvieran en el endpoint, cualquier otro punto de 
entrada podría saltárselas y no se podrían probar sin levantar la API.

DUDAS :

Eliminación: si se quita un futbolista de una plantilla, 
¿se borra el detalle o se marca como inactivo? El plan no pide esta operación.

¿Qué hacemos con las fechas en las que un jugador no puntuó? ¿Es mejor dejar la tabla vacía para esa fecha y 
sumar cero directamente en el cálculo, o deberíamos insertar una fila con puntaje.

El diagrama de clases o modelo inicial, estan en la carpeta llamada "Doc"



 

