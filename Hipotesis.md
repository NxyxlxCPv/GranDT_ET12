CONSIGNA :

Imaginá que un usuario quiere agregar un futbolista a su plantilla y consultar el puntaje de una fecha. Describí qué
objetos intervienen, qué datos guardarías y qué podría salir mal. Dibujá un primer modelo de clases y anotá al
menos tres hipótesis sobre dónde validarías esas situaciones. Registrá también tus dudas. Todavía no
implementes la solución definitiva.
● Considerá titulares, suplentes, presupuesto y puntuaciones por fecha.
● Distinguí datos propios de un futbolista de datos propios de una plantilla.

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

Primer Modelo de clases : 

 

