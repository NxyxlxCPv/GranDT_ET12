-- TRIGGERSSS ---------------------------------------------------------------------------------------------

-- 1. Valida el rango de cotización 
 DROP TRIGGER IF EXISTS BIFlimit;
DELIMITER //
CREATE TRIGGER BIFlimit
BEFORE INSERT ON Futbolista
FOR EACH ROW
BEGIN

IF NEW.Cotizacion < 0 OR NEW.Cotizacion > 99999999.99 THEN
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'Cotización fuera de rango permitido (0 a 99.999.999,99)';
    END IF;
    
END //
DELIMITER ;

-- 2.  Valida el tipo de jugador
 DROP TRIGGER IF EXISTS BITlimit;
DELIMITER //
CREATE TRIGGER BITlimit
BEFORE INSERT ON Futbolista
FOR EACH ROW
BEGIN
IF NEW.Tipo NOT IN ('Arquero','Defensor','Mediocampista','Delantero') THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Tipo de Futbolista inválido (solo se permiten: Arquero, Defensor, Mediocampista, Delantero)';
    END IF;
    
END //
DELIMITER ;


-- 3. Validacion del rango del  puntaje

 DROP TRIGGER IF EXISTS  befInsPuntuacionChk;
DELIMITER //
CREATE TRIGGER  befInsPuntuacionChk
BEFORE INSERT ON PuntuacionFecha
FOR EACH ROW
BEGIN

IF NEW.Puntaje < 1 OR NEW.Puntaje > 10 THEN 
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La puntuación debe estar entre 1 y 10';
    END IF;
    
END //
DELIMITER ;


-- 4. Valida la cantidad de jugadores por plantilla

 DROP TRIGGER IF EXISTS   BIDetallePlantillaLimite;
DELIMITER //
CREATE TRIGGER   BIDetallePlantillaLimite
BEFORE INSERT ON DetallePlantilla
FOR EACH ROW
BEGIN

DECLARE totalJugadores INT;
    DECLARE maxJugadores INT;

    -- Obtener la cantidad máxima permitida para esta plantilla
    SELECT CantidadMaximaJugadores INTO maxJugadores 
    FROM Plantilla 
    WHERE Id = NEW.PlantillaId;

    -- Contar cuántos jugadores tiene actualmente la plantilla
    SELECT COUNT(*) INTO totalJugadores 
    FROM DetallePlantilla 
    WHERE PlantillaId = NEW.PlantillaId;

    -- Validar si supera el límite
    IF totalJugadores >= maxJugadores THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La plantilla ha alcanzado el límite máximo de jugadores permitidos';
    END IF;
    
END //



