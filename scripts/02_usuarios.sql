USE GranDT_ET12;

-- Usuario administrador de ejemplo.
-- Antes de ejecutar el script, reemplazar el correo, la fecha y la contraseña
-- por valores locales. No versionar credenciales reales.
INSERT INTO Usuario (Nombre, Apellido, Email, FechaNacimiento, Password, EsAdministrador)
SELECT 'Admin', 'Ejemplo', 'admin.ejemplo@example.com', '2000-01-01',
       SHA2('CambiarEstaPassword', 256), TRUE
WHERE NOT EXISTS (
    SELECT 1
    FROM Usuario
    WHERE Email = 'admin.ejemplo@example.com'
);
