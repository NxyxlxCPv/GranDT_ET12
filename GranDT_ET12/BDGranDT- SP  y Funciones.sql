-- Seleccionar la base de datos
USE GranDT_ET12;

-- Necesario para poder crear procedimientos/funciones que terminan con ';'
DELIMITER //

-- =========================================================================
-- PROCEDIMIENTOS ALMACENADOS (STORED PROCEDURES)
-- =========================================================================

DROP PROCEDURE IF EXISTS SP_RegistrarUsuario //
CREATE PROCEDURE SP_RegistrarUsuario(
    IN p_Nombre VARCHAR(50),
    IN p_Apellido VARCHAR(50),
    IN p_Email VARCHAR(100),
    IN p_FechaNacimiento DATE,
    IN p_Password VARCHAR(255),
    IN p_EsAdministrador BOOLEAN
)
BEGIN
    INSERT INTO Usuario (Nombre, Apellido, Email, FechaNacimiento, Password, EsAdministrador)
    VALUES (p_Nombre, p_Apellido, p_Email, p_FechaNacimiento, p_Password, p_EsAdministrador);
    
    SELECT LAST_INSERT_ID();
END //

DROP PROCEDURE IF EXISTS SP_RegistrarFutbolista //
CREATE PROCEDURE SP_RegistrarFutbolista(
    IN p_Nombre VARCHAR(50),
    IN p_Apellido VARCHAR(50),
    IN p_Apodo VARCHAR(50),
    IN p_FechaNacimiento DATE,
    IN p_Tipo VARCHAR(20),
    IN p_Cotizacion DECIMAL(12, 2),
    IN p_EquipoId INT
)
BEGIN
    INSERT INTO Futbolista (Nombre, Apellido, Apodo, FechaNacimiento, Tipo, Cotizacion, EquipoId)
    VALUES (p_Nombre, p_Apellido, p_Apodo, p_FechaNacimiento, p_Tipo, p_Cotizacion, p_EquipoId);
    
    SELECT LAST_INSERT_ID();
END //

DROP PROCEDURE IF EXISTS SP_RegistrarPlantilla //
CREATE PROCEDURE SP_RegistrarPlantilla(
    IN p_UsuarioId INT,
    IN p_PresupuestoMaximo DECIMAL(12, 2),
    IN p_CantidadMaximaJugadores INT
)
BEGIN
    INSERT INTO Plantilla (UsuarioId, PresupuestoMaximo, CantidadMaximaJugadores)
    VALUES (p_UsuarioId, p_PresupuestoMaximo, p_CantidadMaximaJugadores);
    
    SELECT LAST_INSERT_ID();
END //

DROP PROCEDURE IF EXISTS SP_AgregarJugadorPlantilla //
CREATE PROCEDURE SP_AgregarJugadorPlantilla(
    IN p_PlantillaId INT,
    IN p_FutbolistaId INT,
    IN p_EsTitular BOOLEAN
)
BEGIN
    INSERT INTO DetallePlantilla (PlantillaId, FutbolistaId, EsTitular)
    VALUES (p_PlantillaId, p_FutbolistaId, p_EsTitular);
END //

DROP PROCEDURE IF EXISTS SP_RegistrarPuntuacion //
CREATE PROCEDURE SP_RegistrarPuntuacion(
    IN p_FutbolistaId INT,
    IN p_NumeroFecha INT,
    IN p_Puntaje DECIMAL(4, 2)
)
BEGIN
    INSERT INTO PuntuacionFecha (FutbolistaId, NumeroFecha, Puntaje)
    VALUES (p_FutbolistaId, p_NumeroFecha, p_Puntaje);
    
    SELECT LAST_INSERT_ID();
END //


-- =========================================================================
-- FUNCIONES (FUNCTIONS)
-- =========================================================================

DROP FUNCTION IF EXISTS FN_CalcularGastoPlantilla //
CREATE FUNCTION FN_CalcularGastoPlantilla(p_PlantillaId INT) 
RETURNS DECIMAL(12,2)
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE total DECIMAL(12,2);
    
    SELECT IFNULL(SUM(f.Cotizacion), 0) INTO total
    FROM DetallePlantilla dp
    INNER JOIN Futbolista f ON dp.FutbolistaId = f.Id
    WHERE dp.PlantillaId = p_PlantillaId;
    
    RETURN total;
END //

DROP FUNCTION IF EXISTS FN_CalcularPuntuacionPlantillaFecha //
CREATE FUNCTION FN_CalcularPuntuacionPlantillaFecha(p_PlantillaId INT, p_NumeroFecha INT)
RETURNS DECIMAL(6,2)
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE totalPuntos DECIMAL(6,2);
    
    SELECT IFNULL(SUM(pf.Puntaje), 0) INTO totalPuntos
    FROM DetallePlantilla dp
    INNER JOIN PuntuacionFecha pf ON dp.FutbolistaId = pf.FutbolistaId
    WHERE dp.PlantillaId = p_PlantillaId AND pf.NumeroFecha = p_NumeroFecha;
    
    RETURN totalPuntos;
END //

DROP FUNCTION IF EXISTS FN_ContarFutbolistas //
CREATE FUNCTION FN_ContarFutbolistas()
RETURNS INT
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE total INT;
    
    SELECT COUNT(*) INTO total
    FROM Futbolista;
    
    RETURN total;
END //

DELIMITER ;