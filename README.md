Gran DT - Documentación y Guía de Ejecución 

Este repositorio contiene el código fuente, la estructura de base de datos, los respaldos y la documentación correspondiente al proyecto Gran DT de la Escuela Técnica N° 12.

 1. Estructura del Repositorio y Entregables

Los archivos y carpetas clave :

backup/torneo.sql: Respaldo completo de la base de datos anonimizada (esquema, tablas, triggers, funciones y datos iniciales de futbolistas y equipos).

scripts/02_usuarios.sql: Script idempotente para la creación de usuarios y gestión de privilegios del sistema.

doc/restauracion.md: Registro detallado de la comprobación y prueba de restauración ejecutada sobre una instancia limpia.

README.md: Guía general de configuración, ejecución de scripts y despliegue local.

 2. Seguridad, Privacidad y Credenciales

Anonimización de PII: Se han sanitizado todos los registros sensibles en la tabla usuario dentro de backup/torneo.sql, reemplazando cualquier dato de identificación personal real por valores ficticios y genéricos.

Credenciales de Ejemplo: Las contraseñas incluidas en los scripts (PasswordAdminEjemplo123!) son estrictamente referenciales para el desarrollo local. Es obligatorio reemplazarlas por claves robustas y seguras antes de realizar cualquier implementación en entornos productivos.

 3. Orden de Ejecución de Scripts SQL

Para inicializar la base de datos desde cero en un servidor MySQL local, se debe seguir estrictamente este orden de ejecución:

Estructura y Esquema Base: Asegurarse de contar con la base de datos creada:

CREATE DATABASE IF NOT EXISTS GranDT_ET12 CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE GranDT_ET12;

Creación de Usuarios y Permisos (scripts/02_usuarios.sql):

CREATE USER IF NOT EXISTS 'admin_gran_dt'@'localhost' IDENTIFIED BY 'PasswordAdminEjemplo123!';
GRANT ALL PRIVILEGES ON GranDT_ET12.* TO 'admin_gran_dt'@'localhost';

CREATE USER IF NOT EXISTS 'invitado_gran_dt'@'localhost' IDENTIFIED BY 'PasswordInvitadoEjemplo123!';
GRANT SELECT ON GranDT_ET12.* TO 'invitado_gran_dt'@'localhost';

FLUSH PRIVILEGES;

Restauración del Respaldo Operativo (backup/torneo.sql): Carga los datos maestros de equipos, futbolistas, triggers y funciones almacenadas.

🔄 4. Instrucciones para la Restauración del Backup

Opción A: Mediante la Consola de Comandos (CMD / Terminal)

Abrir la terminal y ejecutar el comando mysqldump o mysql apuntando al archivo de respaldo:

mysql -u root -p GranDT_ET12 < backup/torneo.sql

Ingresar la contraseña de root cuando sea solicitada.

Opción B: Mediante MySQL Workbench (Interfaz Gráfica)

Abrir MySQL Workbench y conectarse a la instancia local.

Ir al menú superior: Server > Data Import.

Seleccionar la opción Import from Self-Contained File y buscar la ruta del archivo backup/torneo.sql.

En Default Target Schema, seleccionar la base de datos correspondiente (GranDT_ET12).

Hacer clic en Start Import y verificar que el proceso finalice sin errores.

🚀 5. Puesta en Marcha de la API y Ejecución de Pruebas

Configuración de Conexión: Verificar que el archivo de configuración del proyecto en C# (appsettings.json) contenga la cadena de conexión correcta hacia el servidor local MySQL.

Apertura de la Solución: Abrir el archivo principal de la solución GranDT_ET12.slnx en Visual Studio.

Compilación y Ejecución: Presionar la tecla F5 (o hacer clic en el botón Start) para compilar la solución y levantar la aplicación/API.

Ejecución de Pruebas:

Dirigirse a Prueba > Ventanas > Explorador de pruebas (Test Explorer) en Visual Studio.

Ejecutar todas las pruebas unitarias e de integración para validar el correcto funcionamiento de las reglas de negocio, validaciones de presupuesto y triggers de la base de datos.