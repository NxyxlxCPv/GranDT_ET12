
Contrato de API — El Gran DT

GET /api/equipos → 200, lista de equipos.

[
  {
    "id": 1,
    "nombre": "Boca Juniors"
  },
  {
    "id": 2,
    "nombre": "River Plate"
  }
]


GET /api/equipos → 200, lista de equipos.

[
  {
    "id": 10,
    "nombre": "Lionel",
    "apellido": "Messi",
    "apodo": "La Pulga",
    "tipo": "Delantero",
    "cotizacion": 50000000.00,
    "equipoId": 1,
    "equipoNombre": "Inter Miami"
  }
]

POST /api/futbolistas → 201, futbolista creado; 400 o 409 si corresponde.

{
  "nombre": "Emiliano",
  "apellido": "Martínez",
  "apodo": "Dibu",
  "fechaNacimiento": "1992-09-02",
  "tipo": "Arquero",
  "cotizacion": 25000000.00,
  "equipoId": 1
}

Respuesta (201 Created):

{
  "id": 15,
  "nombre": "Emiliano",
  "apellido": "Martínez",
  "apodo": "Dibu",
  "tipo": "Arquero",
  "cotizacion": 25000000.00,
  "equipoId": 1
}

Códigos de error:

400 Bad Request: Datos de entrada inválidos (cotización menor o igual a 0, puesto incorrecto, campos faltantes).

409 Conflict: Se alcanzó el límite máximo de futbolistas configurado en el torneo.

PUT /api/futbolistas/{id} → 200, futbolista con equipo y cotización actualizados; 400 si los datos son
inválidos, 404 si no existe el futbolista.

{
  "nombre": "Emiliano",
  "apellido": "Martínez",
  "apodo": "Dibu",
  "fechaNacimiento": "1992-09-02",
  "tipo": "Arquero",
  "cotizacion": 30000000.00,
  "equipoId": 2
}


Respuesta (200 OK):

{
  "id": 15,
  "nombre": "Emiliano",
  "apellido": "Martínez",
  "apodo": "Dibu",
  "tipo": "Arquero",
  "cotizacion": 30000000.00,
  "equipoId": 2
}


Códigos de error:

400 Bad Request: Datos inválidos o incoherentes en la petición.

404 Not Found: No existe un futbolista con el ID especificado.

POST /api/usuarios → 201, usuario creado sin exponer su contraseña; 400 o 409 si corresponde.

{
  "nombre": "Nayeli",
  "apellido": "Chavez",
  "email": "nayeli@example.com",
  "fechaNacimiento": "2008-05-10",
  "password": "PasswordSegura123"
}


Respuesta (201 Created):

{
  "id": 5,
  "nombre": "Nayeli",
  "apellido": "Chavez",
  "email": "nayeli@example.com",
  "fechaNacimiento": "2008-05-10",
  "esAdministrador": false
}


Códigos de error:

400 Bad Request: Nombre, apellido, email o contraseña vacíos.

409 Conflict: Ya existe un usuario registrado con ese email.

POST /api/futbolistas/{id}/puntuaciones → 201, puntuación registrada; 400, 404 o 409 si corresponde.

{
  "numeroFecha": 4,
  "puntaje": 8.5
}


Respuesta (201 Created):

{
  "id": 102,
  "futbolistaId": 15,
  "numeroFecha": 4,
  "puntaje": 8.5
}


Códigos de error:

400 Bad Request: Número de fecha fuera del rango (1 a 49) o puntaje fuera de escala (1 a 10).

404 Not Found: No existe el futbolista con el id indicado.

409 Conflict: El futbolista ya posee una puntuación asignada para esa fecha.


GET /api/usuarios/{id}/plantillas → 200, lista de plantillas; 404 si no existe el usuario.

Respuesta (200 OK):

[
  {
    "id": 3,
    "usuarioId": 5,
    "presupuestoMaximo": 100000000.00,
    "cantidadMaximaJugadores": 20
  }
]


Códigos de error:

404 Not Found: No existe un usuario registrado con el id indicado.

POST /api/usuarios/{id}/plantillas → 201, plantilla creada; 400 o 404 si corresponde.

Respuesta (201 Created):

{
  "id": 3,
  "usuarioId": 5,
  "presupuestoMaximo": 100000000.00,
  "cantidadMaximaJugadores": 20
}


Códigos de error:

400 Bad Request: Solicitud mal formada.

404 Not Found: El usuario especificado en la ruta no existe.

POST /api/plantillas/{id}/jugadores → 200, plantilla actualizada; 400, 404 o 409 si corresponde.

{
  "futbolistaId": 10,
  "esTitular": true
}


Respuesta (200 OK):

{
  "plantillaId": 3,
  "futbolistaId": 10,
  "esTitular": true,
  "mensaje": "Jugador incorporado a la plantilla exitosamente."
}


Códigos de error:

400 Bad Request: Presupuesto máximo (100.000.000,00) excedido o se alcanzó el límite de 20 jugadores.

404 Not Found: La plantilla o el futbolista no existen.

409 Conflict: El futbolista ya forma parte de esta plantilla.

PUT /api/plantillas/{id}/jugadores/{futbolistaId} → 200, condición de titular o suplente actualizada; 400 si el
dato es inválido, 404 si no existe la plantilla o el futbolista no pertenece a ella.

{
  "esTitular": false
}


Respuesta (200 OK):

{
  "plantillaId": 3,
  "futbolistaId": 10,
  "esTitular": false,
  "mensaje": "Condición del jugador actualizada a suplente."
}


Códigos de error:

400 Bad Request: Valor inválido o la distribución resultante desobedece el esquema táctico del torneo (1-4-4-2).

404 Not Found: La plantilla no existe o el futbolista especificado no pertenece a dicha plantilla.
