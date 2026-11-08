# Contrato de API — El Gran DT

> Proyecto: **El Gran DT** — Web API .NET 10 + Minimal API + MySQL/Dapper
> Entregable: `doc/contrato-api.md`
> Alcance: **10 endpoints** de la etapa de indagación para equipos, futbolistas, usuarios, puntuaciones y plantillas con sus jugadores/titulares.

---

## 1. Convenciones generales

| Tema | Decisión |
|---|---|
| Formato | JSON (`application/json; charset=utf-8`). |
| Nombres de propiedades | `camelCase` (comportamiento por defecto de ASP.NET Core). |
| Fechas | `DateOnly` en formato ISO `yyyy-MM-dd`, por ejemplo `"2008-05-10"`. |
| Decimales | Punto decimal, sin separador de miles: `99999999.99`. |
| Enumeraciones | Se envían y reciben como texto: `"Arquero"`, `"Defensor"`, `"Mediocampista"`, `"Delantero"`. |
| Identificadores | Enteros positivos. |
| Contraseña | Solo se recibe al registrar un usuario. **Nunca** se devuelve en una respuesta. |
| Errores | Formato uniforme: `{ "error": "mensaje legible" }`. |
| Creación exitosa | `201 Created` con cabecera `Location` cuando corresponda. |
| Consulta exitosa | `200 OK`. |

### 1.1 Códigos de estado usados

| Código | Significado | Cuándo se usa |
|---|---|---|
| **200** | OK | Consulta exitosa o actualización sin creación de recurso. |
| **201** | Created | Se creó un recurso nuevo. |
| **400** | Bad Request | Datos inválidos: campos obligatorios, rangos, tipos, reglas simples. |
| **404** | Not Found | El recurso solicitado o una referencia necesaria no existe. |
| **409** | Conflict | Duplicado o conflicto con el estado actual: futbolista repetido, email repetido, puntaje duplicado, límite alcanzado. |
| 500 | Internal Server Error | Error no controlado. No forma parte del contrato funcional, pero debe devolver JSON. |

---

## 2. Resumen de los 10 endpoints

| # | Método | Ruta | Propósito | Éxito | Errores posibles |
|---|---|---|---|---|---|
| 1 | `POST` | `/equipos` | Crear un equipo. | 201 | 400, 409 |
| 2 | `POST` | `/futbolistas` | Crear un futbolista. | 201 | 400, 404, 409 |
| 3 | `GET` | `/futbolistas` | Listar o filtrar futbolistas. | 200 | 400 |
| 4 | `POST` | `/usuarios` | Registrar un usuario. | 201 | 400, 409 |
| 5 | `GET` | `/usuarios/{id}` | Obtener un usuario por Id. | 200 | 404 |
| 6 | `POST` | `/puntuaciones` | Cargar el puntaje de un futbolista en una fecha. | 201 | 400, 404, 409 |
| 7 | `GET` | `/puntuaciones/futbolista/{futbolistaId}` | Ver el historial de puntajes de un futbolista. | 200 | 404 |
| 8 | `POST` | `/usuarios/{usuarioId}/plantillas` | Crear una plantilla para un usuario. | 201 | 400, 404 |
| 9 | `POST` | `/plantillas/{plantillaId}/jugadores` | Agregar un jugador a una plantilla (titular o suplente). | 201 | 400, 404, 409 |
| 10 | `GET` | `/plantillas/{plantillaId}/jugadores` | Listar los jugadores y titulares de una plantilla. | 200 | 404 |

---

## 3. DTOs del contrato

Los DTOs son objetos de la capa API. No son el modelo de dominio: sirven para definir qué entra y qué sale por HTTP. Así evitamos exponer `Password` o aceptar `EsAdministrador` desde el cliente.

```csharp
using GranDT_ET12.Modelos;

// ---------- Equipos ----------
public sealed record CrearEquipoRequest(string Nombre);

public sealed record EquipoResponse(int Id, string Nombre);

// ---------- Futbolistas ----------
public sealed record CrearFutbolistaRequest(
    string Nombre,
    string Apellido,
    string? Apodo,
    DateOnly FechaNacimiento,
    TipoFutbolista Tipo,
    decimal Cotizacion,
    int EquipoId);

public sealed record FutbolistaResponse(
    int Id,
    string Nombre,
    string Apellido,
    string? Apodo,
    DateOnly FechaNacimiento,
    TipoFutbolista Tipo,
    decimal Cotizacion,
    int EquipoId);

// ---------- Usuarios ----------
public sealed record RegistrarUsuarioRequest(
    string Nombre,
    string Apellido,
    string Email,
    DateOnly FechaNacimiento,
    string Password);

public sealed record UsuarioResponse(
    int Id,
    string Nombre,
    string Apellido,
    string Email,
    DateOnly FechaNacimiento,
    bool EsAdministrador);

// ---------- Puntuaciones ----------
public sealed record CargarPuntuacionRequest(
    int FutbolistaId,
    int NumeroFecha,
    decimal Puntaje);

public sealed record PuntuacionResponse(
    int Id,
    int FutbolistaId,
    int NumeroFecha,
    decimal Puntaje);

// ---------- Plantillas ----------
public sealed record CrearPlantillaResponse(
    int Id,
    int UsuarioId,
    decimal PresupuestoMaximo,
    int CantidadMaximaJugadores);

public sealed record AgregarJugadorRequest(
    int FutbolistaId,
    bool EsTitular);

public sealed record JugadorPlantillaResponse(
    int FutbolistaId,
    string Nombre,
    string Apellido,
    TipoFutbolista Tipo,
    decimal Cotizacion,
    bool EsTitular);

// ---------- Error ----------
public sealed record ErrorResponse(string Error);
```

> En la implementación real, estos DTOs pueden vivir en `GranDT_ET12API/Dtos` o en un proyecto `Contracts`. Lo importante es que la API no reciba directamente las clases de `Modelos` cuando eso permitiría enviar campos que no debería controlar el cliente.

---

## 4. Detalle de cada endpoint

### 4.1 `POST /equipos`

Crea un equipo. El nombre es obligatorio y no puede repetirse.

**Entrada:** `CrearEquipoRequest`

```json
{
  "nombre": "River Plate"
}
```

**Respuesta `201 Created`:**

```http
HTTP/1.1 201 Created
Location: /equipos/1
Content-Type: application/json; charset=utf-8

{
  "id": 1,
  "nombre": "River Plate"
}
```

**Códigos:**

| Código | Motivo |
|---|---|
| 201 | Equipo creado. |
| 400 | `nombre` vacío o inválido. |
| 409 | Ya existe un equipo con ese nombre o se alcanzó el máximo de 32 equipos. |

---

### 4.2 `POST /futbolistas`

Crea un futbolista asociado a un equipo existente.

**Entrada:** `CrearFutbolistaRequest`

```json
{
  "nombre": "Lionel",
  "apellido": "Messi",
  "apodo": "La Pulga",
  "fechaNacimiento": "1987-06-24",
  "tipo": "Delantero",
  "cotizacion": 45000000.00,
  "equipoId": 1
}
```

**Respuesta `201 Created`:**

```http
HTTP/1.1 201 Created
Location: /futbolistas/10
Content-Type: application/json; charset=utf-8

{
  "id": 10
}
```

**Códigos:**

| Código | Motivo |
|---|---|
| 201 | Futbolista creado. |
| 400 | Nombre/apellido vacío, `tipo` inválido, cotización fuera del rango `0 < cotización <= 99999999.99`. |
| 404 | El `equipoId` indicado no existe. |
| 409 | Se alcanzó el máximo de 1.500 futbolistas. |

**Reglas asociadas:**

- Cotización máxima: **99.999.999,99**.
- El equipo debe existir.
- El tipo debe ser uno de los cuatro valores del enum.

---

### 4.3 `GET /futbolistas`

Lista todos los futbolistas o aplica filtros opcionales por equipo y por tipo.

**Query params:**

| Parámetro | Tipo | Obligatorio | Ejemplo |
|---|---|---|---|
| `equipoId` | entero | No | `1` |
| `tipo` | texto | No | `Delantero` |

Ejemplos:

```http
GET /futbolistas
GET /futbolistas?equipoId=1
GET /futbolistas?tipo=Delantero
GET /futbolistas?equipoId=1&tipo=Delantero
```

**Respuesta `200 OK`:**

```json
[
  {
    "id": 10,
    "nombre": "Lionel",
    "apellido": "Messi",
    "apodo": "La Pulga",
    "fechaNacimiento": "1987-06-24",
    "tipo": "Delantero",
    "cotizacion": 45000000.00,
    "equipoId": 1
  }
]
```

**Códigos:**

| Código | Motivo |
|---|---|
| 200 | Consulta exitosa. Si no hay resultados, se devuelve `[]`. |
| 400 | `tipo` no es un valor válido del enum. |

---

### 4.4 `POST /usuarios`

Registra un usuario común. El cliente no puede elegir `EsAdministrador`.

**Entrada:** `RegistrarUsuarioRequest`

```json
{
  "nombre": "Nayeli",
  "apellido": "Chavez",
  "email": "nayeli@example.com",
  "fechaNacimiento": "2008-05-10",
  "password": "ClaveSegura123"
}
```

**Respuesta `201 Created`:**

```http
HTTP/1.1 201 Created
Location: /usuarios/3
Content-Type: application/json; charset=utf-8

{
  "id": 3
}
```

**Códigos:**

| Código | Motivo |
|---|---|
| 201 | Usuario registrado. |
| 400 | Nombre, apellido, email o contraseña vacíos; fecha inválida. |
| 409 | Ya existe un usuario con ese email o se alcanzó el máximo de 2.000 usuarios. |

**Notas:**

- La contraseña se encripta en el servicio y nunca se devuelve.
- El registro siempre crea un usuario común (`EsAdministrador = false`).

---

### 4.5 `GET /usuarios/{id}`

Obtiene los datos públicos de un usuario.

**Path param:** `id` (entero).

**Respuesta `200 OK`:**

```json
{
  "id": 3,
  "nombre": "Nayeli",
  "apellido": "Chavez",
  "email": "nayeli@example.com",
  "fechaNacimiento": "2008-05-10",
  "esAdministrador": false
}
```

**Códigos:**

| Código | Motivo |
|---|---|
| 200 | Usuario encontrado. |
| 404 | El usuario no existe. |

---

### 4.6 `POST /puntuaciones`

Carga el puntaje de un futbolista en una fecha del torneo.

**Entrada:** `CargarPuntuacionRequest`

```json
{
  "futbolistaId": 10,
  "numeroFecha": 4,
  "puntaje": 8.5
}
```

**Respuesta `201 Created`:**

```http
HTTP/1.1 201 Created
Location: /puntuaciones/100
Content-Type: application/json; charset=utf-8

{
  "id": 100
}
```

**Códigos:**

| Código | Motivo |
|---|---|
| 201 | Puntuación cargada. |
| 400 | `numeroFecha` fuera de 1–49 o `puntaje` fuera de 1–10. |
| 404 | El `futbolistaId` no existe. |
| 409 | El futbolista ya tiene una puntuación en esa fecha. |

**Reglas asociadas:**

- `NumeroFecha` entre **1** y **49**.
- `Puntaje` entre **1** y **10**.
- Un solo puntaje por futbolista y fecha.

---

### 4.7 `GET /puntuaciones/futbolista/{futbolistaId}`

Devuelve el historial de puntajes de un futbolista.

**Path param:** `futbolistaId` (entero).

**Respuesta `200 OK`:**

```json
[
  {
    "id": 100,
    "futbolistaId": 10,
    "numeroFecha": 4,
    "puntaje": 8.5
  },
  {
    "id": 101,
    "futbolistaId": 10,
    "numeroFecha": 5,
    "puntaje": 7.0
  }
]
```

**Códigos:**

| Código | Motivo |
|---|---|
| 200 | Consulta exitosa. Si el futbolista existe pero no tiene puntajes, se devuelve `[]`. |
| 404 | El futbolista no existe. |

**Nota:** las fechas sin puntaje no se inventan. Si no hay fila para una fecha, esa fecha no aparece en el historial. La decisión de mostrar “sin puntaje” corresponde a la capa que consuma la API.

---

### 4.8 `POST /usuarios/{usuarioId}/plantillas`

Crea una plantilla vacía para un usuario. El presupuesto y el máximo de jugadores se toman de las reglas del sistema.

**Path param:** `usuarioId` (entero).

**Entrada:** sin cuerpo o `{}`.

**Respuesta `201 Created`:**

```http
HTTP/1.1 201 Created
Location: /plantillas/5
Content-Type: application/json; charset=utf-8

{
  "id": 5,
  "usuarioId": 3,
  "presupuestoMaximo": 100000000.00,
  "cantidadMaximaJugadores": 20
}
```

**Códigos:**

| Código | Motivo |
|---|---|
| 201 | Plantilla creada. |
| 400 | `usuarioId` inválido. |
| 404 | El usuario no existe. |

**Notas:**

- Un usuario puede tener más de una plantilla.
- La plantilla nace vacía; los jugadores se agregan con el endpoint 9.

---

### 4.9 `POST /plantillas/{plantillaId}/jugadores`

Agrega un futbolista a una plantilla e indica si es titular o suplente.

**Path param:** `plantillaId` (entero).

**Entrada:** `AgregarJugadorRequest`

```json
{
  "futbolistaId": 10,
  "esTitular": true
}
```

**Respuesta `201 Created`:**

```http
HTTP/1.1 201 Created
Location: /plantillas/5/jugadores/10
Content-Type: application/json; charset=utf-8

{
  "plantillaId": 5,
  "futbolistaId": 10,
  "esTitular": true
}
```

**Códigos:**

| Código | Motivo |
|---|---|
| 201 | Jugador agregado. |
| 400 | Se supera el presupuesto, se supera el máximo de 20 jugadores o la composición de titulares no es 1-4-4-2. |
| 404 | La plantilla o el futbolista no existen. |
| 409 | El futbolista ya se encuentra en la plantilla. |

**Ejemplos de error `400`:**

```json
{
  "error": "El presupuesto máximo de la plantilla ha sido superado."
}
```

```json
{
  "error": "La plantilla ya tiene el máximo de 20 jugadores."
}
```

```json
{
  "error": "Los titulares deben ser 1 arquero, 4 defensores, 4 mediocampistas y 2 delanteros."
}
```

**Ejemplo de error `409`:**

```json
{
  "error": "El futbolista ya se encuentra en la plantilla."
}
```

**Reglas asociadas:**

- Suma de cotizaciones de todos los jugadores **≤ 100.000.000,00**.
- Máximo **20** jugadores por plantilla.
- Un futbolista no puede repetirse en la misma plantilla.
- Si `esTitular` es `true`, la composición debe respetar **1 arquero, 4 defensores, 4 mediocampistas y 2 delanteros**.
- Los suplentes no cuentan para la composición de titulares, pero sí para el máximo de 20 y para el presupuesto.

---

### 4.10 `GET /plantillas/{plantillaId}/jugadores`

Lista los jugadores de una plantilla, incluyendo sus datos y si son titulares.

**Path param:** `plantillaId` (entero).

**Respuesta `200 OK`:**

```json
[
  {
    "futbolistaId": 10,
    "nombre": "Lionel",
    "apellido": "Messi",
    "tipo": "Delantero",
    "cotizacion": 45000000.00,
    "esTitular": true
  },
  {
    "futbolistaId": 11,
    "nombre": "Emiliano",
    "apellido": "Martínez",
    "tipo": "Arquero",
    "cotizacion": 20000000.00,
    "esTitular": true
  },
  {
    "futbolistaId": 12,
    "nombre": "Cristian",
    "apellido": "Romero",
    "tipo": "Defensor",
    "cotizacion": 15000000.00,
    "esTitular": true
  }
]
```

**Códigos:**

| Código | Motivo |
|---|---|
| 200 | Consulta exitosa. Si la plantilla existe pero está vacía, se devuelve `[]`. |
| 404 | La plantilla no existe. |

**Notas:**

- El campo `esTitular` permite a la interfaz separar titulares de suplentes.
- El total gastado se puede calcular sumando `cotizacion` en el cliente o exponerlo en un endpoint de resumen en una etapa posterior.

---

## 5. Matriz de reglas por endpoint

| Endpoint | Reglas principales |
|---|---|
| `POST /equipos` | Nombre obligatorio, único; máximo 32 equipos. |
| `POST /futbolistas` | Nombre y apellido obligatorios; cotización `0 < c <= 99.999.999,99`; equipo existente; máximo 1.500 futbolistas. |
| `GET /futbolistas` | Filtros opcionales; tipo válido. |
| `POST /usuarios` | Nombre, apellido, email y contraseña obligatorios; email único; máximo 2.000 usuarios. |
| `GET /usuarios/{id}` | El usuario debe existir. |
| `POST /puntuaciones` | Fecha 1–49; puntaje 1–10; futbolista existente; sin duplicado futbolista+fecha. |
| `GET /puntuaciones/futbolista/{id}` | El futbolista debe existir. |
| `POST /usuarios/{id}/plantillas` | El usuario debe existir; presupuesto 100.000.000 y máximo 20 por defecto. |
| `POST /plantillas/{id}/jugadores` | Presupuesto, máximo 20, sin repetido, composición 1-4-4-2 para titulares. |
| `GET /plantillas/{id}/jugadores` | La plantilla debe existir. |

---

## 6. Flujo completo de ejemplo

Este flujo muestra cómo se encadenan los 10 endpoints para armar una plantilla y cargar puntajes.

```text
1. POST /equipos
   { "nombre": "River Plate" }
   → 201 { "id": 1 }

2. POST /futbolistas
   { "nombre": "Lionel", "apellido": "Messi", ..., "equipoId": 1 }
   → 201 { "id": 10 }

3. GET /futbolistas?equipoId=1
   → 200 [ { "id": 10, ... } ]

4. POST /usuarios
   { "nombre": "Nayeli", ..., "password": "ClaveSegura123" }
   → 201 { "id": 3 }

5. GET /usuarios/3
   → 200 { "id": 3, "nombre": "Nayeli", ... }

6. POST /usuarios/3/plantillas
   → 201 { "id": 5, "usuarioId": 3, "presupuestoMaximo": 100000000.00, ... }

7. POST /plantillas/5/jugadores
   { "futbolistaId": 10, "esTitular": true }
   → 201 { "plantillaId": 5, "futbolistaId": 10, "esTitular": true }

8. GET /plantillas/5/jugadores
   → 200 [ { "futbolistaId": 10, "esTitular": true, ... } ]

9. POST /puntuaciones
   { "futbolistaId": 10, "numeroFecha": 4, "puntaje": 8.5 }
   → 201 { "id": 100 }

10. GET /puntuaciones/futbolista/10
    → 200 [ { "id": 100, "futbolistaId": 10, "numeroFecha": 4, "puntaje": 8.5 } ]
```

---

## 7. Notas de implementación

1. **Minimal API:** usar `Results.Created`, `Results.Ok`, `Results.BadRequest`, `Results.NotFound` y `Results.Conflict` para que los códigos sean explícitos.
2. **Validación:** las reglas de negocio van en los servicios; la API solo valida la forma del contrato y traduce excepciones.
3. **Excepciones:** `ReglaNegocioException` se puede mapear a `400` o `409` según el caso. Un duplicado debería ser `409`; un rango inválido, `400`.
4. **Contraseña:** nunca incluir `Password` en un DTO de respuesta. El modelo ya usa `[JsonIgnore]`, pero es mejor no depender solo de eso.
5. **Presupuesto:** unificar la constante en **100.000.000,00** para que coincida con este contrato.
6. **Swagger:** documentar cada endpoint con `WithSummary`, `WithTags` y ejemplos. La configuración básica está en `doc/contraste.md`.
7. **Formato de error:** mantener siempre `{ "error": "..." }`. Si más adelante se adopta `ProblemDetails`, debe ser un cambio consciente y documentado.
