# Restauración del respaldo `backup/torneo.sql`

Este documento registra la comprobación de restauración del respaldo de la base
`GranDT_ET12` en una base temporal vacía llamada `GranDT_ET12_test`.

## 1. Entorno utilizado

- Sistema operativo: Windows.
- MySQL Server: 8.0.46 (`C:\Program Files\MySQL\MySQL Server 8.0\bin`).
- SDK de .NET: 10.0.401.
- Respaldo: `backup/torneo.sql` (30.970 bytes).
- Base temporal: `GranDT_ET12_test`.

La instancia MySQL local no tenía disponible la credencial de `root` configurada
en el proyecto, por lo que la comprobación se realizó sobre una instancia
temporal aislada de MySQL 8.0.46 en `127.0.0.1:33307`, inicializada con
`--initialize-insecure`. Los comandos de creación, importación y verificación son
los mismos que se usan contra la instancia local; solo cambian el host y el
puerto.

## 2. Advertencia sobre datos personales

Antes de restaurar se revisó el contenido de `backup/torneo.sql`. El respaldo
incluye:

- Un registro en la tabla `Usuario` con nombre, apellido, correo y fecha de
  nacimiento que parecen corresponder a una persona real.
- Un hash SHA-256 en el campo `Password` de ese mismo registro.
- Los nombres y fechas de nacimiento de futbolistas profesionales en la tabla
  `Futbolista` (datos públicos).

No se encontraron contraseñas de servidor ni cadenas de conexión dentro del
respaldo. Antes de publicar o compartir el respaldo se recomienda reemplazar los
datos del usuario por valores ficticios y cambiar el hash por el de una
contraseña de ejemplo.

## 3. Comandos ejecutados

### 3.1. Crear la base temporal vacía

```powershell
$mysql = 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe'

& $mysql --no-defaults --host=127.0.0.1 --port=33307 --user=root `
    --execute="DROP DATABASE IF EXISTS GranDT_ET12_test; CREATE DATABASE GranDT_ET12_test CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;"
```

Equivalente contra la instancia local:

```powershell
& $mysql -u root -p --execute="DROP DATABASE IF EXISTS GranDT_ET12_test; CREATE DATABASE GranDT_ET12_test CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;"
```

### 3.2. Importar el respaldo

```powershell
& $mysql --no-defaults --host=127.0.0.1 --port=33307 --user=root `
    --default-character-set=utf8mb4 GranDT_ET12_test `
    --execute="source C:/Repos/GranDT_ET12/backup/torneo.sql"
```

Equivalente contra la instancia local:

```powershell
& $mysql -u root -p --default-character-set=utf8mb4 GranDT_ET12_test `
    --execute="source C:/Repos/GranDT_ET12/backup/torneo.sql"
```

También se puede usar la redirección desde `cmd.exe`:

```bat
"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe" -u root -p GranDT_ET12_test < backup\torneo.sql
```

### 3.3. Verificar tablas, registros y triggers

```powershell
& $mysql --no-defaults --host=127.0.0.1 --port=33307 --user=root `
    --default-character-set=utf8mb4 --table GranDT_ET12_test `
    --execute="SHOW TABLES; SELECT 'DetallePlantilla' AS Tabla, COUNT(*) AS Registros FROM DetallePlantilla UNION ALL SELECT 'Equipo', COUNT(*) FROM Equipo UNION ALL SELECT 'Futbolista', COUNT(*) FROM Futbolista UNION ALL SELECT 'Plantilla', COUNT(*) FROM Plantilla UNION ALL SELECT 'PuntuacionFecha', COUNT(*) FROM PuntuacionFecha UNION ALL SELECT 'Usuario', COUNT(*) FROM Usuario; SHOW TRIGGERS;"
```

## 4. Resultados obtenidos

La importación terminó con código de salida `0` y se crearon las 6 tablas
esperadas:

| Tabla             | Registros |
| ----------------- | --------: |
| `DetallePlantilla` |         0 |
| `Equipo`           |         5 |
| `Futbolista`       |        73 |
| `Plantilla`        |         0 |
| `PuntuacionFecha`  |         0 |
| `Usuario`          |         1 |

`SHOW TRIGGERS` devolvió los 4 triggers esperados, todos `BEFORE INSERT`:

| Trigger                    | Tabla             | Momento        |
| -------------------------- | ----------------- | -------------- |
| `BIDetallePlantillaLimite` | `detalleplantilla` | `BEFORE INSERT` |
| `BIFlimit`                 | `futbolista`       | `BEFORE INSERT` |
| `BITlimit`                 | `futbolista`       | `BEFORE INSERT` |
| `befInsPuntuacionChk`      | `puntuacionfecha`  | `BEFORE INSERT` |

Además, el respaldo restauró correctamente las rutinas almacenadas que
contiene: 5 funciones (`FN_CalcularGastoPlantilla`,
`FN_CalcularPuntuacionPlantillaFecha`, `FN_ContarFutbolistas`,
`FN_ExisteEquipo`, `FN_ExisteFutbolista`) y 5 procedimientos
(`SP_AgregarJugadorPlantilla`, `SP_RegistrarFutbolista`,
`SP_RegistrarPlantilla`, `SP_RegistrarPuntuacion`, `SP_RegistrarUsuario`).

## 5. Limpieza

Finalizada la comprobación se eliminó la base temporal y se detuvo la instancia
aislada:

```powershell
& $mysql --no-defaults --host=127.0.0.1 --port=33307 --user=root `
    --execute="DROP DATABASE GranDT_ET12_test; SHOW DATABASES LIKE 'GranDT_ET12_test';"

& 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqladmin.exe' `
    --no-defaults --host=127.0.0.1 --port=33307 --user=root shutdown
```

La consulta posterior a `SHOW DATABASES LIKE 'GranDT_ET12_test'` no devolvió
filas, por lo que la base temporal quedó eliminada. También se eliminó el
directorio temporal usado por la instancia aislada.

## 6. Observaciones

- El respaldo **no incluye** la sentencia `CREATE DATABASE` ni `USE`; por eso la
  base de destino debe crearse vacía antes de importarlo.
- Los triggers y rutinas se restauraron con `DEFINER=root@localhost`. Si se
  restaura en un servidor donde ese usuario no existe, puede ser necesario
  crearlo o regenerar el respaldo sin `DEFINER`.
- Para generar un nuevo respaldo con la base incluida se recomienda:

```powershell
& 'C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe' -u root -p `
    --routines --triggers --single-transaction --databases GranDT_ET12 `
    > backup\torneo.sql
```

Con `--databases`, el nuevo archivo incluirá `CREATE DATABASE` y `USE`, por lo
que podrá restaurarse directamente sin crear la base manualmente.
