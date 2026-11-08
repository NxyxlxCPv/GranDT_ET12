# Etapa de indagación — Hipótesis y modelo inicial

> Proyecto: **El Gran DT** — Web API .NET 10 + Minimal API + MySQL/Dapper
> Entregable: `doc/hipotesis.md`
> Objetivo: dejar escrito qué objetos intervienen, qué reglas los afectan y dónde suponemos que conviene validarlas antes de escribir la versión definitiva.

---

## 1. Propósito de esta etapa

En la indagación no buscamos todavía la solución perfecta. Buscamos **entender el problema**, proponer un primer modelo de objetos y dejar explícitas las dudas que después se van a contrastar con la práctica.

La idea central es separar tres preguntas:

1. **¿Qué datos existen?** → modelo de objetos y tablas.
2. **¿Qué reglas deben cumplirse?** → reglas de negocio.
3. **¿Dónde se controla cada regla?** → hipótesis de validación.

---

## 2. Modelo inicial de objetos

### 2.1 Entidades y relaciones

| Objeto | Qué representa | Datos principales |
|---|---|---|
| **Usuario** | Persona registrada que puede tener una o más plantillas. | Id, nombre, apellido, email, fecha de nacimiento, contraseña encriptada, es administrador. |
| **Equipo** | Club al que pertenece un futbolista. | Id, nombre. |
| **Futbolista** | Jugador con datos propios, puesto y cotización. | Id, nombre, apellido, apodo, fecha de nacimiento, tipo, cotización, equipo. |
| **Plantilla** | Equipo fantasy de un usuario. | Id, usuario, presupuesto máximo, cantidad máxima de jugadores. |
| **DetallePlantilla** | Relación entre una plantilla y un futbolista. Indica si es titular o suplente. | PlantillaId, FutbolistaId, esTitular. |
| **PuntuacionFecha** | Puntaje obtenido por un futbolista en una fecha del torneo. | Id, futbolista, número de fecha, puntaje. |

Relaciones:

- Un **Usuario** tiene **muchas Plantillas** (1 a N).
- Un **Equipo** tiene **muchos Futbolistas** (1 a N).
- Una **Plantilla** tiene **muchos Futbolistas** y un Futbolista puede estar en muchas Plantillas (N a N), resuelto con **DetallePlantilla**.
- Un **Futbolista** tiene **muchas PuntuacionFecha** (1 a N), una por cada fecha en la que fue calificado.

### 2.2 Código del modelo en C#

```csharp
namespace GranDT_ET12.Modelos;

public enum TipoFutbolista
{
    Arquero,
    Defensor,
    Mediocampista,
    Delantero
}

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly FechaNacimiento { get; set; }
    public string Password { get; set; } = string.Empty; // encriptada, nunca se devuelve en JSON
    public bool EsAdministrador { get; set; }
}

public class Equipo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class Futbolista
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Apodo { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public TipoFutbolista Tipo { get; set; }
    public decimal Cotizacion { get; set; } // hasta 99.999.999,99
    public int EquipoId { get; set; }
}

public class Plantilla
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public decimal PresupuestoMaximo { get; set; } = 100_000_000.00m;
    public int CantidadMaximaJugadores { get; set; } = 20;
}

public class DetallePlantilla
{
    public int PlantillaId { get; set; }
    public int FutbolistaId { get; set; }
    public bool EsTitular { get; set; }
}

public class PuntuacionFecha
{
    public int Id { get; set; }
    public int FutbolistaId { get; set; }
    public int NumeroFecha { get; set; } // 1 a 49
    public decimal Puntaje { get; set; } // 1 a 10
}
```

### 2.3 Reglas de negocio del modelo

| Regla | Valor / condición | ¿Qué la hace especial? |
|---|---|---|
| Cotización de un futbolista | Mayor a 0 y menor o igual a **99.999.999,99** | Es un rango de un solo dato. |
| Presupuesto de una plantilla | Máximo **100.000.000,00** | Depende de la suma de las cotizaciones de todos los jugadores. |
| Cantidad de jugadores | Hasta **20** por plantilla | Depende del detalle actual de la plantilla. |
| Futbolista repetido | Un futbolista no puede aparecer dos veces en la misma plantilla | Depende de la relación Plantilla–Futbolista. |
| Composición de titulares | **1 arquero, 4 defensores, 4 mediocampistas y 2 delanteros** | Depende del tipo de cada titular. |
| Puntaje | Entre **1** y **10** | Es un rango de un solo dato. |
| Número de fecha | Entre **1** y **49** | Es un rango de un solo dato. |
| Puntuación duplicada | Un futbolista no puede tener dos puntajes en la misma fecha | Depende de la combinación futbolista + fecha. |
| Nombre y apellido | Obligatorios | Es una validación de un solo objeto. |
| Email de usuario | Obligatorio y único | Requiere consultar si ya existe otro usuario con ese email. |

### 2.4 Modelo relacional mínimo

Las reglas importantes no deberían vivir solamente en C#. La base de datos también puede defender la integridad con claves, `UNIQUE` y `CHECK`.

```sql
CREATE TABLE Usuario (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    FechaNacimiento DATE NOT NULL,
    Password CHAR(64) NOT NULL,
    EsAdministrador BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE Equipo (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Futbolista (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Apodo VARCHAR(100) NULL,
    FechaNacimiento DATE NOT NULL,
    Tipo ENUM('Arquero', 'Defensor', 'Mediocampista', 'Delantero') NOT NULL,
    Cotizacion DECIMAL(10, 2) NOT NULL,
    EquipoId INT NOT NULL,
    CONSTRAINT FK_Futbolista_Equipo FOREIGN KEY (EquipoId) REFERENCES Equipo(Id),
    CONSTRAINT CK_Futbolista_Cotizacion
        CHECK (Cotizacion > 0 AND Cotizacion <= 99999999.99)
);

CREATE TABLE Plantilla (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    PresupuestoMaximo DECIMAL(12, 2) NOT NULL DEFAULT 100000000.00,
    CantidadMaximaJugadores INT NOT NULL DEFAULT 20,
    CONSTRAINT FK_Plantilla_Usuario FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id),
    CONSTRAINT CK_Plantilla_Presupuesto
        CHECK (PresupuestoMaximo > 0 AND PresupuestoMaximo <= 100000000.00),
    CONSTRAINT CK_Plantilla_Cantidad
        CHECK (CantidadMaximaJugadores > 0 AND CantidadMaximaJugadores <= 20)
);

CREATE TABLE DetallePlantilla (
    PlantillaId INT NOT NULL,
    FutbolistaId INT NOT NULL,
    EsTitular BOOLEAN NOT NULL DEFAULT FALSE,
    PRIMARY KEY (PlantillaId, FutbolistaId),
    CONSTRAINT FK_Detalle_Plantilla FOREIGN KEY (PlantillaId) REFERENCES Plantilla(Id),
    CONSTRAINT FK_Detalle_Futbolista FOREIGN KEY (FutbolistaId) REFERENCES Futbolista(Id)
);

CREATE TABLE PuntuacionFecha (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FutbolistaId INT NOT NULL,
    NumeroFecha INT NOT NULL,
    Puntaje DECIMAL(4, 2) NOT NULL,
    CONSTRAINT FK_Puntuacion_Futbolista FOREIGN KEY (FutbolistaId) REFERENCES Futbolista(Id),
    CONSTRAINT UQ_Puntuacion_Futbolista_Fecha UNIQUE (FutbolistaId, NumeroFecha),
    CONSTRAINT CK_Puntuacion_Fecha CHECK (NumeroFecha BETWEEN 1 AND 49),
    CONSTRAINT CK_Puntuacion_Puntaje CHECK (Puntaje BETWEEN 1 AND 10)
);
```

> **Detalle importante:** `DECIMAL(10,2)` alcanza justo para 99.999.999,99. En cambio, el presupuesto de 100.000.000,00 necesita `DECIMAL(12,2)` porque tiene nueve dígitos enteros.

---

## 3. Hipótesis sobre dónde validar las reglas

Una hipótesis no es una verdad definitiva: es una suposición que después vamos a probar con código y con pruebas. Estas son las cuatro hipótesis iniciales.

### Hipótesis 1 — Las reglas de un solo objeto se validan en el servicio

**Suposición:** si una regla solamente necesita los datos del objeto que se está por guardar, no hace falta consultar la base de datos. El servicio la rechaza antes de tocar MySQL.

Ejemplos: cotización dentro de rango, puntaje entre 1 y 10, fecha entre 1 y 49, nombre obligatorio.

```csharp
public int Cargar(PuntuacionFecha puntuacion)
{
    if (puntuacion.NumeroFecha < 1 || puntuacion.NumeroFecha > 49)
        throw new ReglaNegocioException("La fecha debe estar entre 1 y 49.");

    if (puntuacion.Puntaje < 1 || puntuacion.Puntaje > 10)
        throw new ReglaNegocioException("El puntaje debe estar entre 1 y 10.");

    if (_futbolistaRepositorio.ObtenerPorId(puntuacion.FutbolistaId) is null)
        throw new ReglaNegocioException("El futbolista indicado no existe.");

    if (_puntuacionRepositorio.ExistePuntuacionEnFecha(
            puntuacion.FutbolistaId, puntuacion.NumeroFecha))
        throw new ReglaNegocioException("El futbolista ya tiene una puntuación en esa fecha.");

    return _puntuacionRepositorio.InsertarPuntuacion(puntuacion);
}
```

**Qué probaría:** que el endpoint responda `400` sin ejecutar un `INSERT` cuando el dato es inválido.
**Cómo lo verificaría:** prueba unitaria del servicio con un repositorio simulado; no debería llamarse al repositorio de escritura.

---

### Hipótesis 2 — Las reglas que dependen de varios objetos se validan en el servicio de plantillas

**Suposición:** presupuesto, máximo de 20 jugadores, futbolista repetido y composición de titulares necesitan información de la plantilla, de sus detalles y de cada futbolista. Ningún objeto aislado tiene todos esos datos. Por eso los reúne `PlantillaService`, valida y recién después manda a guardar.

```csharp
public void AgregarJugador(int plantillaId, int futbolistaId, bool esTitular)
{
    Plantilla? plantilla = _plantillaRepositorio.ObtenerPorId(plantillaId);
    if (plantilla is null)
        throw new ReglaNegocioException("La plantilla indicada no existe.");

    Futbolista? futbolista = _futbolistaRepositorio.ObtenerPorId(futbolistaId);
    if (futbolista is null)
        throw new ReglaNegocioException("El futbolista indicado no existe.");

    List<DetallePlantilla> detalles =
        _detalleRepositorio.ObtenerPorPlantillaId(plantillaId).ToList();

    // Regla 1: no repetir futbolista.
    if (detalles.Any(d => d.FutbolistaId == futbolistaId))
        throw new ReglaNegocioException("El futbolista ya se encuentra en la plantilla.");

    // Regla 2: máximo 20 jugadores.
    if (detalles.Count >= plantilla.CantidadMaximaJugadores)
        throw new ReglaNegocioException(
            $"La plantilla ya tiene el máximo de {plantilla.CantidadMaximaJugadores} jugadores.");

    // Regla 3: presupuesto.
    decimal gastoActual = detalles.Sum(d =>
        _futbolistaRepositorio.ObtenerPorId(d.FutbolistaId)!.Cotizacion);

    if (gastoActual + futbolista.Cotizacion > plantilla.PresupuestoMaximo)
        throw new ReglaNegocioException(
            "El presupuesto máximo de la plantilla ha sido superado.");

    // Regla 4: composición de titulares (se puede validar al agregar
    // o al confirmar la plantilla; ver dudas más abajo).
    if (esTitular)
        ValidarComposicionTitulares(detalles, futbolista);

    _detalleRepositorio.AgregarJugador(new DetallePlantilla
    {
        PlantillaId = plantillaId,
        FutbolistaId = futbolistaId,
        EsTitular = esTitular
    });
}

private static void ValidarComposicionTitulares(
    List<DetallePlantilla> detalles,
    Futbolista nuevo)
{
    // Se obtienen los tipos de los titulares actuales y se agrega el nuevo.
    // La idea es que nunca haya más de 1 arquero, 4 defensores,
    // 4 mediocampistas o 2 delanteros entre los titulares.
    // La implementación concreta depende de cómo se consulten los futbolistas.
}
```

**Qué probaría:** que el servicio rechace el alta antes del `INSERT` y que el mensaje explique la regla incumplida.
**Cómo lo verificaría:** pruebas unitarias con listas de detalles simuladas: 20 jugadores, presupuesto al límite, jugador repetido y titulares incompletos.

---

### Hipótesis 3 — El endpoint no decide reglas: solo traduce entre HTTP y el servicio

**Suposición:** la capa API recibe los datos, llama al servicio y convierte el resultado en un código de estado. No calcula presupuestos ni consulta tablas.

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

**Qué probaría:** que las mismas reglas se puedan usar desde otro punto de entrada (por ejemplo, una consola o una prueba) sin depender de HTTP.
**Cómo lo verificaría:** reutilizar el servicio en una prueba unitaria y comprobar que el endpoint solamente mapea resultados a `201`, `400`, `404` o `409`.

---

### Hipótesis 4 — La base de datos actúa como última defensa, no como único lugar de validación

**Suposición:** aunque el servicio valide, la base también debe impedir datos incoherentes. La validación en C# da mensajes claros y se prueba rápido; las restricciones de MySQL protegen ante errores, carreras entre pedidos o accesos directos.

```sql
-- Evita el futbolista repetido incluso si dos pedidos llegan al mismo tiempo.
ALTER TABLE DetallePlantilla
    ADD CONSTRAINT UQ_DetallePlantilla_Futbolista
    UNIQUE (PlantillaId, FutbolistaId);

-- Evita dos puntajes del mismo futbolista en la misma fecha.
ALTER TABLE PuntuacionFecha
    ADD CONSTRAINT UQ_Puntuacion_Futbolista_Fecha
    UNIQUE (FutbolistaId, NumeroFecha);
```

**Qué probaría:** que un `INSERT` directo o concurrente no pueda romper la regla.
**Cómo lo verificaría:** prueba de integración que intente insertar dos veces el mismo par y espere un error de clave duplicada.

> Conclusión provisional: **no es “servicio o base de datos”, sino servicio + base de datos**. Cada uno cubre una necesidad distinta.

---

## 4. Dudas iniciales sobre la separación de responsabilidades

Estas son preguntas que todavía no tienen una respuesta cerrada. Dejarlas escritas ayuda a no mezclar capas por accidente.

| Duda | Criterio provisional para la indagación |
|---|---|
| ¿El modelo debe tener comportamiento o solo datos? | Empezar con un modelo simple (datos) y servicios que contengan las reglas. Si una regla es propia del objeto, evaluar moverla al modelo. |
| ¿Quién calcula el gasto total de la plantilla? | El servicio de plantillas, porque es una regla de negocio. El repositorio solo trae los datos. Más adelante se puede usar una función SQL por eficiencia. |
| ¿La composición 1-4-4-2 se valida al agregar cada jugador o al confirmar la plantilla? | Provisionalmente al confirmar, para permitir armar la plantilla de a poco. Si se valida al agregar, el usuario no puede probar combinaciones temporales. |
| ¿Quitar un jugador borra el detalle o lo marca inactivo? | Provisionalmente borrado físico del detalle. Las puntuaciones históricas no se borran porque pertenecen al futbolista, no a la plantilla. |
| ¿Qué pasa si falta la puntuación de una fecha? | Provisionalmente la ausencia de fila significa “sin puntaje”. No inventar un 0, porque 0 no es lo mismo que “no jugó” o “no se cargó”. |
| ¿La autorización se controla en la API, en el servicio o en MySQL? | La API autentica; el servicio verifica que el usuario sea dueño de la plantilla; MySQL limita privilegios y protege la integridad. Son responsabilidades distintas. |
| ¿La API devuelve modelos o DTOs? | DTOs. Así no se expone `Password`, no se acepta `EsAdministrador` desde el cliente y el contrato no queda atado al modelo interno. |
| ¿Dónde van las transacciones? | En la infraestructura o en el servicio, según la operación. Una operación que toca varias tablas debe ser atómica. |
| ¿Dónde se encripta la contraseña? | En el servicio de usuarios o en un componente de seguridad. Nunca en el controlador ni en el repositorio. |
| ¿Cómo se evita que dos pedidos agreguen el jugador 20 al mismo tiempo? | Con la restricción `UNIQUE` y una transacción. El servicio puede avisar, pero la base es la que garantiza. |
| ¿El presupuesto se guarda en la plantilla o se lee de una constante? | Guardarlo en la plantilla permite cambiarlo por usuario o por torneo. La constante solo define el valor inicial. |

---

## 5. Observaciones para la implementación

1. **Presupuesto:** el enunciado de esta etapa fija **100.000.000,00**. Si el código actual tiene otro valor en `Constantes.PresupuestoMaximoPlantilla`, hay que unificarlo para que el documento y la implementación coincidan.
2. **Titulares:** la composición **1-4-4-2** está definida en las constantes, pero todavía falta usarla en `PlantillaService`. Es una de las reglas a implementar después de la indagación.
3. **Puntuación faltante:** conviene decidir si la API devuelve `null`, una lista vacía o un estado “sin puntaje”. La decisión debe quedar documentada porque afecta los cálculos.
4. **Contraseña:** el proyecto usa SHA-256 para generar un hash de la contraseña. Para un sistema real se recomienda una función pensada para contraseñas (PBKDF2, bcrypt o Argon2), pero para la etapa alcanza con dejar claro que nunca se devuelve en JSON.

---

## 6. Resumen de la hipótesis

- El **modelo** describe los datos y sus relaciones.
- Los **servicios** concentran las reglas de negocio y las validaciones que dependen de varios objetos.
- La **API** solo traduce HTTP ↔ servicio.
- La **base de datos** refuerza la integridad con claves, `UNIQUE` y `CHECK`.
- Las dudas quedan abiertas para contrastarlas en `doc/contraste.md`.
