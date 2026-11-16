# GranDT_ET12

Escuela: Escuela Técnica N° 12 D.E. 1° "Libertador Gral. José de San Martín"
Especialidad: Computación
Materias: Programación Orientada a Objetos, Base de Datos, Análisis de Sistemas
Profesores: Sergio Mendoza, Luis Duran, Lorenzo Polverino
Integrantes:

- Nayeli Chavez
- Ruth Lezcano

## Descripción general

GranDT es una Web API en .NET 10 para administrar equipos, futbolistas,
plantillas y puntuaciones. La persistencia se realiza en MySQL 8.0 mediante
Dapper y MySqlConnector.

## Estructura del repositorio

- `scripts/`: scripts SQL de creación, triggers e inserción de datos.
- `backup/torneo.sql`: respaldo de la base `GranDT_ET12`.
- `GranDT_ET12/`: biblioteca con modelos, repositorios y servicios.
- `GranDT_ET12API/`: proyecto de la Web API en .NET 10.
- `GranDT_ET12.Tests/`: proyecto de pruebas con xUnit.
- `doc/restauracion.md`: comprobación paso a paso de la restauración del respaldo.

## Requisitos previos

- MySQL Server 8.0 o superior.
- .NET 10 SDK.
- Cliente de MySQL (`mysql.exe` y `mysqldump.exe`). En Windows, la ruta habitual
  es `C:\Program Files\MySQL\MySQL Server 8.0\bin`.

Verificación rápida:

```powershell
& "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe" --version
dotnet --version
```

## Orden de ejecución de los scripts SQL

Ejecutar los scripts desde la raíz del repositorio y en este orden:

1. `scripts/00 DDL.sql`: crea la base `GranDT_ET12` y las 6 tablas.
2. `scripts/02 Triggers.sql`: crea los 4 triggers de validación.
3. `scripts/01 Inserts.sql`: carga equipos, futbolistas y un usuario de ejemplo.
4. `scripts/02_usuarios.sql`: carga usuarios adicionales.

Los triggers se ejecutan antes de las inserciones para que validen los datos que
carga `01 Inserts.sql`.

> `02_usuarios.sql` inserta un administrador de ejemplo solo si su correo no
> existe todavía. Reemplazar sus datos antes de ejecutarlo.

Comandos para ejecutar cada script con el cliente de MySQL:

```powershell
$mysql = "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe"

& $mysql -u root -p --execute="source scripts/00 DDL.sql"
& $mysql -u root -p GranDT_ET12 --execute="source scripts/02 Triggers.sql"
& $mysql -u root -p GranDT_ET12 --execute="source scripts/01 Inserts.sql"
& $mysql -u root -p GranDT_ET12 --execute="source scripts/02_usuarios.sql"
```

> Cada script se ejecuta en una invocación separada. A partir de los triggers se
> indica `GranDT_ET12` como base porque `02 Triggers.sql` no contiene una
> sentencia `USE`.
>
> El script `02_usuarios.sql` es idempotente para el correo de ejemplo: si se
> cambia el correo, actualizar también la condición `NOT EXISTS`.

## Contraseñas de ejemplo

Antes de ejecutar `01 Inserts.sql` y `02_usuarios.sql`, reemplazar localmente
los correos, fechas de nacimiento y hashes de contraseña de ejemplo por datos
ficticios. No usar contraseñas reales en archivos versionados.

El campo `Password` almacena un hash SHA-256 en hexadecimal. Para generar el
hash de una contraseña de prueba desde PowerShell:

```powershell
$texto = "MiPasswordDePrueba"
$sha = [System.Security.Cryptography.SHA256]::Create()
$hash = [BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($texto))).Replace("-", "").ToLower()
$hash
```

Luego reemplazar el valor correspondiente en el `INSERT` del script. Si se
modifica un script para pruebas locales, no incluir esos cambios en el commit.

## Cadena de conexión

La cadena de conexión se encuentra en
`GranDT_ET12/Repositorios/BdConexion.cs:6`. Actualmente está escrita en el
código con un usuario y una contraseña de ejemplo.

Para trabajar localmente:

1. Reemplazar en ese archivo el usuario y la contraseña por los de tu instancia
   de MySQL.
2. No agregar ese cambio al repositorio. Antes de commitear, revisar:

```powershell
git diff -- GranDT_ET12/Repositorios/BdConexion.cs
git status
```

3. Como mejora pendiente, mover la cadena a una variable de entorno o a
   `dotnet user-secrets` para no versionar credenciales. Ese cambio requiere
   modificar el código de configuración.

Las pruebas de repositorio también contienen cadenas de conexión locales en
`GranDT_ET12.Tests/Repositorios/`; si se ejecutan los tests, ajustarlas
localmente sin subir credenciales.

## Restaurar el respaldo en una base vacía

El archivo `backup/torneo.sql` no incluye `CREATE DATABASE`, por lo que primero
hay que crear la base de destino:

```powershell
$mysql = "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe"

& $mysql -u root -p --execute="DROP DATABASE IF EXISTS GranDT_ET12_test; CREATE DATABASE GranDT_ET12_test CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;"
& $mysql -u root -p --default-character-set=utf8mb4 GranDT_ET12_test --execute="source backup/torneo.sql"
```

Verificar la restauración:

```powershell
& $mysql -u root -p --table GranDT_ET12_test --execute="SHOW TABLES; SHOW TRIGGERS; SELECT COUNT(*) AS Futbolistas FROM Futbolista;"
```

Eliminar la base temporal al terminar:

```powershell
& $mysql -u root -p --execute="DROP DATABASE GranDT_ET12_test;"
```

El detalle de la comprobación realizada, con los resultados obtenidos, está en
`doc/restauracion.md`.

## Generar un nuevo respaldo

```powershell
& "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe" -u root -p `
    --routines --triggers --single-transaction --databases GranDT_ET12 `
    > backup/torneo.sql
```

Con `--databases`, el nuevo respaldo incluye `CREATE DATABASE` y `USE`, por lo
que puede restaurarse directamente en una instancia vacía.

## Ejecutar la Web API

```powershell
dotnet run --project GranDT_ET12API/GranDT_ET12API.csproj
```

La documentación Swagger queda disponible en `http://localhost:5118/swagger`.

## Ejecutar las pruebas

```powershell
dotnet test GranDT_ET12.Tests/GranDT_ET12.Tests.csproj
```

Los tests de repositorio requieren una base MySQL local accesible con la cadena
de conexión configurada en los archivos de prueba.
