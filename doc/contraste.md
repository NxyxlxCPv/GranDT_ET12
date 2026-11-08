# Contraste técnico — modelo, servicios, infraestructura y API

> Proyecto: **El Gran DT** — Web API .NET 10 + Minimal API + MySQL/Dapper
> Entregable: `doc/contraste.md`
> Objetivo: contrastar las hipótesis de la indagación con una separación clara de responsabilidades y responder las preguntas guía con fundamentos técnicos.

---

## 1. Separación de responsabilidades

### 1.1 Capas del proyecto

| Capa | Proyecto / carpeta | Responsabilidad | Lo que **no** debe hacer |
|---|---|---|---|
| **Modelo / dominio** | `GranDT_ET12/Modelos` | Representar los datos y las relaciones del negocio. | Conectarse a MySQL, leer HTTP o conocer códigos de estado. |
| **Servicios de aplicación** | `GranDT_ET12/Service` | Aplicar reglas de negocio, coordinar repositorios y lanzar errores de negocio. | Escribir SQL, devolver `ActionResult` o conocer Swagger. |
| **Infraestructura** | `GranDT_ET12/Repositorios` | Ejecutar SQL/Dapper, abrir conexiones, llamar procedimientos almacenados. | Decidir si un jugador está repetido o si el presupuesto alcanza. |
| **API / presentación** | `GranDT_ET12API` (Controllers o Minimal API) | Recibir HTTP, validar forma del contrato, llamar al servicio y traducir resultados a códigos de estado. | Calcular reglas de negocio o consultar tablas directamente. |
| **Configuración y seguridad** | `GranDT_ET12/Configuracion`, `GranDT_ET12/Seguridad` | Constantes del sistema, cadena de conexión, encriptación de contraseñas. | Mezclarse con la lógica de los endpoints. |

### 1.2 Dirección de las dependencias

La regla de oro es que las dependencias apunten hacia adentro:

```text
API  ───────►  Servicios  ───────►  Interfaces de repositorio
                                        ▲
                                        │ implementan
                                 Repositorios (Dapper/MySQL)
```

- La API depende de los servicios.
- Los servicios dependen de **interfaces** de repositorio, no de MySQL.
- Los repositorios implementan esas interfaces.
- El modelo es compartido por todas las capas, pero no depende de ninguna.

Esto permite probar los servicios con repositorios simulados, sin levantar MySQL ni la API.

### 1.3 Ejemplo de interfaces y registro de dependencias

```csharp
// Servicio: no sabe si los datos vienen de MySQL, de un archivo o de una prueba.
public class PlantillaService : IPlantillaService
{
    private readonly IPlantillaRepositorio _plantillaRepositorio;
    private readonly IDetallePlantillaRepositorio _detalleRepositorio;
    private readonly IFutbolistaRepositorio _futbolistaRepositorio;

    public PlantillaService(
        IPlantillaRepositorio plantillaRepositorio,
        IDetallePlantillaRepositorio detalleRepositorio,
        IFutbolistaRepositorio futbolistaRepositorio)
    {
        _plantillaRepositorio = plantillaRepositorio;
        _detalleRepositorio = detalleRepositorio;
        _futbolistaRepositorio = futbolistaRepositorio;
    }
}
```

```csharp
// Program.cs: la API arma el grafo de dependencias.
builder.Services.AddScoped<IPlantillaRepositorio>(_ =>
    new PlantillaRepositorio(cadena));

builder.Services.AddScoped<IDetallePlantillaRepositorio>(_ =>
    new DetallePlantillaRepositorio(cadena));

builder.Services.AddScoped<IPlantillaService, PlantillaService>();
```

> En el repositorio actual la presentación usa Controllers. Para la etapa de indagación se pide una prueba de concepto con Minimal API. Ambas son formas de la **misma capa API**: cambia la sintaxis, no la responsabilidad.

---

## 2. Preguntas guía

### 2.1 ¿Cómo se evita repetir un futbolista en una plantilla?

Hay que defender la regla en **tres niveles**, porque cada uno cubre un problema distinto.

#### Nivel 1 — Validación en el servicio (mensaje claro)

Antes de insertar, el servicio consulta el detalle actual de la plantilla:

```csharp
List<DetallePlantilla> detalles =
    _detalleRepositorio.ObtenerPorPlantillaId(plantillaId).ToList();

if (detalles.Any(d => d.FutbolistaId == futbolistaId))
{
    throw new ReglaNegocioException(
        "El futbolista ya se encuentra en la plantilla.");
}
```

Esto permite responder `409 Conflict` con un mensaje entendible para el usuario.

#### Nivel 2 — Restricción en MySQL (integridad garantizada)

La tabla intermedia debe tener una clave primaria o una restricción `UNIQUE` sobre el par:

```sql
CREATE TABLE DetallePlantilla (
    PlantillaId INT NOT NULL,
    FutbolistaId INT NOT NULL,
    EsTitular BOOLEAN NOT NULL DEFAULT FALSE,
    PRIMARY KEY (PlantillaId, FutbolistaId),
    CONSTRAINT FK_Detalle_Plantilla
        FOREIGN KEY (PlantillaId) REFERENCES Plantilla(Id),
    CONSTRAINT FK_Detalle_Futbolista
        FOREIGN KEY (FutbolistaId) REFERENCES Futbolista(Id)
);
```

La clave primaria compuesta ya impide el duplicado. Si se prefiere una tabla con `Id` propio, se agrega:

```sql
ALTER TABLE DetallePlantilla
    ADD CONSTRAINT UQ_DetallePlantilla_Futbolista
    UNIQUE (PlantillaId, FutbolistaId);
```

#### Nivel 3 — Manejo de concurrencia (dos pedidos al mismo tiempo)

Dos usuarios —o dos pestañas del mismo usuario— podrían pasar la validación del servicio a la vez. La base sigue siendo la última defensa. El repositorio puede traducir el error de clave duplicada de MySQL (`1062`) a una regla de negocio:

```csharp
try
{
    _detalleRepositorio.AgregarJugador(nuevo);
}
catch (MySqlException ex) when (ex.Number == 1062)
{
    throw new ReglaNegocioException(
        "El futbolista ya se encuentra en la plantilla.");
}
```

También se puede usar una transacción con `SELECT ... FOR UPDATE` sobre la plantilla para serializar las altas, pero la restricción `UNIQUE` es más simple y siempre debe existir.

**Conclusión:** el servicio evita el caso normal y da un buen mensaje; la base de datos garantiza que la regla no se rompa nunca.

---

### 2.2 ¿Qué pasa si falta su puntuación en una fecha?

Primero hay que definir qué significa “falta”:

| Situación | Interpretación recomendada |
|---|---|
| No existe fila en `PuntuacionFecha` para ese futbolista y esa fecha. | **Sin puntaje cargado** o “no jugó”. No es lo mismo que 0. |
| Existe fila con `Puntaje = 0`. | Puntaje realmente cero, si el reglamento lo permite. |
| La fecha no existe en el torneo. | Error de datos: la fecha debe estar entre 1 y 49. |

#### No inventar un cero

Insertar un `0` automáticamente contamina las estadísticas y oculta que el dato falta. Lo correcto es **no crear la fila** y que la consulta distinga entre “no hay dato” y “el dato vale cero”.

#### Consulta con `LEFT JOIN`

```sql
SELECT
    f.Id           AS FutbolistaId,
    f.Nombre,
    f.Apellido,
    p.Puntaje      AS Puntaje
FROM Futbolista f
LEFT JOIN PuntuacionFecha p
    ON p.FutbolistaId = f.Id
   AND p.NumeroFecha = @numeroFecha
WHERE f.Id = @futbolistaId;
```

Si no hay puntuación, `Puntaje` vuelve `NULL`.

#### DTO con puntaje nullable

```csharp
public sealed record PuntajeFechaDto(
    int FutbolistaId,
    int NumeroFecha,
    decimal? Puntaje); // null = no hay puntaje cargado
```

#### ¿Qué responde la API?

- Si el **futbolista no existe** → `404 Not Found`.
- Si el futbolista existe pero **no tiene puntaje en esa fecha** → `200 OK` con `puntaje: null` o con una lista vacía, según el contrato.
- Si se pide el **historial** de un futbolista → `200 OK` con las fechas que sí tienen puntaje; las faltantes simplemente no aparecen.

#### Si el negocio decide que “sin puntaje = 0”

Entonces debe quedar documentado y aplicarse siempre igual:

```sql
SELECT
    f.Id AS FutbolistaId,
    @numeroFecha AS NumeroFecha,
    COALESCE(p.Puntaje, 0) AS Puntaje
FROM Futbolista f
LEFT JOIN PuntuacionFecha p
    ON p.FutbolistaId = f.Id
   AND p.NumeroFecha = @numeroFecha
WHERE f.Id = @futbolistaId;
```

La diferencia es importante: `COALESCE` **oculta** la ausencia. Solo se usa si el reglamento dice explícitamente que un futbolista sin puntaje suma cero.

---

### 2.3 ¿Qué diferencia hay entre validar datos en los servicios y verificar permisos de usuario a nivel de MySQL?

Son dos problemas distintos. Mezclarlos suele terminar en una base de datos con lógica de negocio o en una API que confía demasiado en sí misma.

| Aspecto | Validación en el servicio | Permisos y restricciones en MySQL |
|---|---|---|
| **Pregunta que responde** | ¿Los datos cumplen las reglas del negocio? | ¿Quién puede leer o modificar qué? |
| **Ejemplos** | Puntaje entre 1 y 10; presupuesto máximo; futbolista repetido. | El usuario de la aplicación solo puede ejecutar ciertos procedimientos; no puede borrar la tabla `Usuario`. |
| **Mensaje al usuario** | Claro y en español: “El presupuesto fue superado”. | Error técnico: “Access denied” o “Duplicate entry”. |
| **Pruebas** | Unitarias, rápidas, sin base de datos. | De integración, con MySQL real o contenedor. |
| **Concurrencia** | No alcanza por sí sola. | `UNIQUE`, `CHECK`, claves foráneas y transacciones. |
| **Mantenimiento** | Cambia con las reglas del torneo. | Cambia con la seguridad y la infraestructura. |

#### MySQL no tiene “row level security” como PostgreSQL

MySQL no ofrece políticas de fila nativas. La autorización por fila se implementa combinando:

1. **Verificación de propiedad en la consulta:**

```sql
UPDATE Plantilla
SET PresupuestoMaximo = @presupuesto
WHERE Id = @plantillaId
  AND UsuarioId = @usuarioId;
```

```csharp
int filasAfectadas = conexion.Execute(sql, new
{
    presupuesto,
    plantillaId,
    usuarioId
});

if (filasAfectadas == 0)
{
    // No existe o no pertenece al usuario.
    // Se responde 404 para no revelar si la plantilla existe.
    throw new ReglaNegocioException("La plantilla no existe o no pertenece al usuario.");
}
```

2. **Usuario de base de datos con privilegios mínimos:**

```sql
CREATE USER 'grando_app'@'localhost' IDENTIFIED BY 'ClaveSegura';

-- Solo lo necesario para la aplicación.
GRANT SELECT, INSERT, UPDATE, DELETE ON GranDT_ET12.* TO 'grando_app'@'localhost';

-- Alternativa más restrictiva: exponer procedimientos y vistas.
GRANT EXECUTE ON PROCEDURE GranDT_ET12.SP_AgregarJugadorPlantilla
    TO 'grando_app'@'localhost';
```

3. **Procedimientos que filtran por usuario:**

```sql
CREATE PROCEDURE SP_ObtenerPlantillasDelUsuario(IN p_UsuarioId INT)
BEGIN
    SELECT Id, UsuarioId, PresupuestoMaximo, CantidadMaximaJugadores
    FROM Plantilla
    WHERE UsuarioId = p_UsuarioId;
END;
```

En la práctica, el `usuarioId` se pasa como parámetro desde la aplicación; MySQL no conoce la sesión web ni puede leer un “usuario logueado” de ASP.NET Core.

#### Lo que **no** conviene poner en MySQL

- La decisión de si un usuario es administrador basada solo en una tabla editable por la app.
- El hash de contraseñas generado con lógica de negocio compleja.
- Mensajes de validación para el usuario final.

#### Lo que **sí** conviene poner en MySQL

- Claves primarias, foráneas, `UNIQUE` y `CHECK`.
- Privilegios mínimos por usuario.
- Procedimientos almacenados para operaciones críticas.
- Transacciones para que una operación no quede a medias.

> **Conclusión:** el servicio valida el negocio; MySQL protege la integridad y limita el daño. Son capas complementarias, no alternativas. Usar `root` en la aplicación —como suele pasar en los primeros prototipos— debe cambiarse antes de producción.

---

## 3. Prueba de concepto: Minimal API que devuelve JSON

La prueba de concepto es un proyecto **descartable**. No reemplaza la arquitectura del proyecto real; solo demuestra que una ruta Minimal API puede devolver JSON y documentarse con Swagger.

### 3.1 Crear el proyecto

```powershell
dotnet new web -n PocGranDT
cd PocGranDT
dotnet add package Swashbuckle.AspNetCore --version 10.2.3
```

### 3.2 Código completo de `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Descubre los endpoints de Minimal API para que Swagger los muestre.
builder.Services.AddEndpointsApiExplorer();

// 2. Genera el documento OpenAPI.
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Publica el JSON de OpenAPI y la interfaz gráfica.
app.UseSwagger();
app.UseSwaggerUI();

// Datos en memoria: la PoC no usa base de datos.
var futbolistas = new List<FutbolistaDto>
{
    new(1, "Lionel", "Messi", "Delantero", 99_999_999.99m),
    new(2, "Emiliano", "Martínez", "Arquero", 45_000_000.00m)
};

app.MapGet("/api/futbolistas/{id:int}", (int id) =>
{
    FutbolistaDto? futbolista = futbolistas.FirstOrDefault(f => f.Id == id);

    return futbolista is null
        ? Results.NotFound(new { error = "El futbolista no existe." })
        : Results.Ok(futbolista);
})
.WithName("ObtenerFutbolista")
.WithSummary("Devuelve un futbolista por Id")
.WithTags("Futbolistas");

app.Run();

// DTO de la PoC. No es el modelo de dominio: es lo que viaja por HTTP.
public sealed record FutbolistaDto(
    int Id,
    string Nombre,
    string Apellido,
    string Tipo,
    decimal Cotizacion);
```

### 3.3 Ejecutar y probar

```powershell
dotnet run
```

Luego:

- Abrir `https://localhost:<puerto>/swagger` (el puerto aparece en la consola o en `launchSettings.json`).
- Probar la ruta desde Swagger con **Try it out**.
- O desde PowerShell:

```powershell
Invoke-RestMethod "http://localhost:5000/api/futbolistas/1" | ConvertTo-Json
```

Respuesta esperada:

```json
{
  "id": 1,
  "nombre": "Lionel",
  "apellido": "Messi",
  "tipo": "Delantero",
  "cotizacion": 99999999.99
}
```

### 3.4 Cómo se configura Swagger

| Instrucción | Para qué sirve |
|---|---|
| `AddEndpointsApiExplorer()` | Hace que los endpoints Minimal API aparezcan en la documentación. Sin esto, Swagger no los “ve”. |
| `AddSwaggerGen()` | Genera el documento OpenAPI (JSON) a partir de las rutas y los DTOs. |
| `UseSwagger()` | Publica el JSON en `/swagger/v1/swagger.json`. |
| `UseSwaggerUI()` | Publica la interfaz visual en `/swagger`. |
| `.WithName(...)`, `.WithSummary(...)`, `.WithTags(...)` | Agregan metadatos legibles a la documentación. |

> En .NET 9 y .NET 10 también existe el soporte nativo de OpenAPI (`AddOpenApi` / `MapOpenApi`). El proyecto ya usa **Swashbuckle 10.2.3**, así que la PoC mantiene la misma herramienta para no introducir otra dependencia.

### 3.5 Qué demuestra la PoC

- Una ruta Minimal API puede devolver JSON sin controladores.
- El código de estado se elige con `Results.Ok` o `Results.NotFound`.
- Swagger se configura con cuatro instrucciones y queda disponible en `/swagger`.
- La ruta no contiene reglas de negocio: solo busca un dato y responde.

---

## 4. Cómo llevar la PoC al proyecto real

En el proyecto real, la ruta no debe buscar datos “a mano”. Debe llamar al servicio y dejar que él aplique las reglas:

```csharp
app.MapGet("/futbolistas/{id:int}", (int id, IFutbolistaService service) =>
{
    Futbolista? futbolista = service.ObtenerPorId(id);

    return futbolista is null
        ? Results.NotFound(new { error = "El futbolista no existe." })
        : Results.Ok(futbolista);
});
```

Para las operaciones de escritura:

```csharp
app.MapPost("/plantillas/{plantillaId:int}/jugadores",
    (int plantillaId, AgregarJugadorRequest request, IPlantillaService service) =>
{
    try
    {
        service.AgregarJugador(plantillaId, request.FutbolistaId, request.EsTitular);
        return Results.Created(
            $"/plantillas/{plantillaId}/jugadores/{request.FutbolistaId}",
            new { plantillaId, request.FutbolistaId, request.EsTitular });
    }
    catch (ReglaNegocioException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});
```

La diferencia entre la PoC y el proyecto real no es la sintaxis de Minimal API: es que el proyecto real **delega las reglas en el servicio** y **persiste en MySQL mediante Dapper**.

---

## 5. Resumen del contraste

- **Modelo:** datos y relaciones.
- **Servicio:** reglas de negocio y coordinación.
- **Repositorio:** SQL/Dapper y acceso a MySQL.
- **API:** contrato HTTP y traducción a códigos de estado.
- **Base de datos:** integridad, restricciones y privilegios mínimos.
- **Swagger:** documentación viva del contrato, no lógica de negocio.
