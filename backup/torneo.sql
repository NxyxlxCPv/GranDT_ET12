-- MySQL dump 10.13  Distrib 8.0.46, for Win64 (x86_64)
--
-- Host: localhost    Database: GranDT_ET12
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `detalleplantilla`
--

DROP TABLE IF EXISTS `detalleplantilla`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalleplantilla` (
  `PlantillaId` int NOT NULL,
  `FutbolistaId` int NOT NULL,
  `EsTitular` tinyint(1) NOT NULL,
  PRIMARY KEY (`PlantillaId`,`FutbolistaId`),
  KEY `FK_DetallePlantilla_Futbolista` (`FutbolistaId`),
  CONSTRAINT `FK_DetallePlantilla_Futbolista` FOREIGN KEY (`FutbolistaId`) REFERENCES `futbolista` (`Id`),
  CONSTRAINT `FK_DetallePlantilla_Plantilla` FOREIGN KEY (`PlantillaId`) REFERENCES `plantilla` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `detalleplantilla`
--

LOCK TABLES `detalleplantilla` WRITE;
/*!40000 ALTER TABLE `detalleplantilla` DISABLE KEYS */;
/*!40000 ALTER TABLE `detalleplantilla` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `BIDetallePlantillaLimite` BEFORE INSERT ON `detalleplantilla` FOR EACH ROW BEGIN

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
   
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `equipo`
--

DROP TABLE IF EXISTS `equipo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `equipo` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nombre` (`Nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `equipo`
--

LOCK TABLES `equipo` WRITE;
/*!40000 ALTER TABLE `equipo` DISABLE KEYS */;
INSERT INTO `equipo` VALUES (2,'Boca Juniors'),(5,'Club Atlético Platense'),(3,'Independiente'),(4,'Racing Club'),(1,'River Plate');
/*!40000 ALTER TABLE `equipo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `futbolista`
--

DROP TABLE IF EXISTS `futbolista`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `futbolista` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Apodo` varchar(50) DEFAULT NULL,
  `FechaNacimiento` date NOT NULL,
  `Tipo` enum('Arquero','Defensor','Mediocampista','Delantero') NOT NULL,
  `Cotizacion` decimal(10,2) NOT NULL,
  `EquipoId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Futbolista_Equipo` (`EquipoId`),
  CONSTRAINT `FK_Futbolista_Equipo` FOREIGN KEY (`EquipoId`) REFERENCES `equipo` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=74 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `futbolista`
--

LOCK TABLES `futbolista` WRITE;
/*!40000 ALTER TABLE `futbolista` DISABLE KEYS */;
INSERT INTO `futbolista` VALUES (1,'Franco','Armani','El Pulpo','1986-10-16','Arquero',15000000.00,1),(2,'Jeremías','Ledesma','Conan','1993-02-13','Arquero',12000000.00,1),(3,'Paulo','Díaz','Paulo','1994-08-25','Defensor',25000000.00,1),(4,'Germán','Pezzella','El Bahiense','1991-06-27','Defensor',20000000.00,1),(5,'Marcos','Acuña','El Huevo','1991-10-28','Defensor',22000000.00,1),(6,'Fabricio','Bustos','El Tractor','1996-04-28','Defensor',18000000.00,1),(7,'Enzo','Díaz','Enzo','1995-09-24','Defensor',16000000.00,1),(8,'Enzo','Pérez','Enzo','1986-02-22','Mediocampista',14000000.00,1),(9,'Rodrigo','Aliendro','Peti','1991-02-16','Mediocampista',15000000.00,1),(10,'Manuel','Lanzini','Manu','1993-02-15','Mediocampista',20000000.00,1),(11,'Ignacio','Fernández','Nacho','1990-01-12','Mediocampista',18000000.00,1),(12,'Claudio','Echeverri','El Diablito','2006-01-02','Mediocampista',30000000.00,1),(13,'Facundo','Colidio','Facu','2000-01-04','Delantero',22000000.00,1),(14,'Miguel','Borja','El Colibrí','1993-01-26','Delantero',28000000.00,1),(15,'Pablo','Solari','Pablito','2001-03-22','Delantero',20000000.00,1),(16,'Sergio','Romero','Chiquito','1987-02-22','Arquero',14000000.00,2),(17,'Leandro','Brey','Lea','2002-09-21','Arquero',10000000.00,2),(18,'Marcos','Rojo','Marquitos','1990-03-20','Defensor',18000000.00,2),(19,'Nicolás','Figal','Nico','1994-04-03','Defensor',16000000.00,2),(20,'Luis','Advíncula','El Rayo','1990-03-02','Defensor',17000000.00,2),(21,'Lautaro','Blanco','Lauti','1999-02-19','Defensor',19000000.00,2),(22,'Aaron','Anselmino','Aaron','2005-04-29','Defensor',25000000.00,2),(23,'Pol','Fernández','Pol','1991-10-11','Mediocampista',15000000.00,2),(24,'Cristian','Medina','Maravilla','2002-06-01','Mediocampista',26000000.00,2),(25,'Kevin','Zenón','Kevin','2001-07-30','Mediocampista',25000000.00,2),(26,'Ignacio','Miramón','Nacho','2003-06-12','Mediocampista',16000000.00,2),(27,'Edinson','Cavani','El Matador','1987-02-14','Delantero',35000000.00,2),(28,'Miguel','Merentiel','La Bestia','1996-02-24','Delantero',30000000.00,2),(29,'Exequiel','Zeballos','El Changuito','2002-04-24','Delantero',20000000.00,2),(30,'Milton','Giménez','Milton','1996-08-12','Delantero',18000000.00,2),(31,'Rodrigo','Rey','Rey','1991-03-08','Arquero',16000000.00,3),(32,'Diego','Segovia','Diego','2000-05-15','Arquero',8000000.00,3),(33,'Joaquín','Laso','Laso','1990-07-04','Defensor',12000000.00,3),(34,'Kevin','Lomónaco','Kevin','2002-01-08','Defensor',18000000.00,3),(35,'Federico','Vera','Fede','1998-03-24','Defensor',14000000.00,3),(36,'Damián','Pérez','Damián','1988-12-22','Defensor',10000000.00,3),(37,'Ivan','Marcone','El Capo','1990-06-03','Mediocampista',15000000.00,3),(38,'Lucas','González','Saltita','2000-06-03','Mediocampista',14000000.00,3),(39,'Felipe','Loyola','Pipe','2000-11-09','Mediocampista',18000000.00,3),(40,'Federico','Mancuello','Mancu','1989-03-26','Mediocampista',12000000.00,3),(41,'Gabriel','Ávalos','El Tanque','1990-10-12','Delantero',20000000.00,3),(42,'Santiago','Hidalgo','Santi','2005-02-17','Delantero',15000000.00,3),(43,'Alexis','Canelo','Canelo','1992-02-03','Delantero',14000000.00,3),(44,'Matías','Giménez','Mati','1999-03-06','Delantero',16000000.00,3),(45,'Gabriel','Arias','Gabi','1987-09-13','Arquero',15000000.00,4),(46,'Facundo','Cambeses','Facu','1997-04-09','Arquero',11000000.00,4),(47,'Leonardo','Sigali','El Oso','1987-05-29','Defensor',12000000.00,4),(48,'Agustín','García Basso','Agus','1992-04-25','Defensor',17000000.00,4),(49,'Marco','Di Cesare','Marco','2002-01-30','Defensor',20000000.00,4),(50,'Gastón','Martirena','Gasti','2000-01-05','Defensor',18000000.00,4),(51,'Gabriel','Rojas','Gabi','1997-06-22','Defensor',16000000.00,4),(52,'Bruno','Zuculini','Zucu','1993-04-02','Mediocampista',15000000.00,4),(53,'Juan Fernando','Quintero','Juanfer','1993-01-18','Mediocampista',25000000.00,4),(54,'Agustín','Almendra','Agus','2000-02-11','Mediocampista',18000000.00,4),(55,'Santiago','Sosa','Santi','1999-05-03','Mediocampista',22000000.00,4),(56,'Adrian','Martínez','Maravilla','1992-07-07','Delantero',28000000.00,4),(57,'Roger','Martínez','Roger','1994-06-23','Delantero',24000000.00,4),(58,'Luciano','Vietto','Lucho','1993-12-05','Delantero',20000000.00,4),(59,'Johan','Carbonero','Johan','1999-07-20','Delantero',19000000.00,4),(60,'Juan Pablo','Cozzani','Juampi','1998-05-09','Arquero',12000000.00,5),(61,'Joaquín','Blázquez','Joaco','2001-01-28','Arquero',9000000.00,5),(62,'Gastón','Suso','Gaston','1991-03-12','Defensor',13000000.00,5),(63,'Ignacio','Vázquez','Nacho','1997-06-15','Defensor',14000000.00,5),(64,'Sasha','Marcich','Sasha','1998-05-29','Defensor',12000000.00,5),(65,'Bautista','Barros Schelotto','Bauti','2000-01-13','Defensor',13000000.00,5),(66,'Carlos','Villalba','Carlitos','1998-07-19','Mediocampista',14000000.00,5),(67,'Fernando','Juárez','Ajito','1998-08-23','Mediocampista',12000000.00,5),(68,'Leonel','Picco','Leo','1998-10-22','Mediocampista',15000000.00,5),(69,'Franco','Mineria','Franco','2002-11-04','Mediocampista',11000000.00,5),(70,'Mateo','Pellegrino','Flaco','2001-10-22','Delantero',18000000.00,5),(71,'Ronaldo','Martínez','Roni','1996-04-25','Delantero',15000000.00,5),(72,'Augusto','Lotti','Lotti','1996-06-10','Delantero',16000000.00,5),(73,'Ignacio','Schor','El Pájaro','2000-08-04','Delantero',14000000.00,5);
/*!40000 ALTER TABLE `futbolista` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `BIFlimit` BEFORE INSERT ON `futbolista` FOR EACH ROW BEGIN

IF NEW.Cotizacion < 0 OR NEW.Cotizacion > 99999999.99 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cotización fuera de rango permitido (0 a 99.999.999,99)';
    END IF;
   
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `BITlimit` BEFORE INSERT ON `futbolista` FOR EACH ROW BEGIN
IF NEW.Tipo NOT IN ('Arquero','Defensor','Mediocampista','Delantero') THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Tipo de Futbolista inválido (solo se permiten: Arquero, Defensor, Mediocampista, Delantero)';
    END IF;
   
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `plantilla`
--

DROP TABLE IF EXISTS `plantilla`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `plantilla` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UsuarioId` int NOT NULL,
  `PresupuestoMaximo` decimal(12,2) NOT NULL,
  `CantidadMaximaJugadores` int NOT NULL DEFAULT '20',
  PRIMARY KEY (`Id`),
  KEY `FK_Plantilla_Usuario` (`UsuarioId`),
  CONSTRAINT `FK_Plantilla_Usuario` FOREIGN KEY (`UsuarioId`) REFERENCES `usuario` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plantilla`
--

LOCK TABLES `plantilla` WRITE;
/*!40000 ALTER TABLE `plantilla` DISABLE KEYS */;
/*!40000 ALTER TABLE `plantilla` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `puntuacionfecha`
--

DROP TABLE IF EXISTS `puntuacionfecha`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `puntuacionfecha` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FutbolistaId` int NOT NULL,
  `NumeroFecha` int NOT NULL,
  `Puntaje` decimal(4,2) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQ_Futbolista_Fecha` (`FutbolistaId`,`NumeroFecha`),
  CONSTRAINT `FK_PuntuacionFecha_Futbolista` FOREIGN KEY (`FutbolistaId`) REFERENCES `futbolista` (`Id`),
  CONSTRAINT `CHK_NumeroFecha` CHECK (((`NumeroFecha` < 50) and (`NumeroFecha` > 0))),
  CONSTRAINT `CHK_Puntaje` CHECK (((`Puntaje` >= 1.00) and (`Puntaje` <= 10.00)))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `puntuacionfecha`
--

LOCK TABLES `puntuacionfecha` WRITE;
/*!40000 ALTER TABLE `puntuacionfecha` DISABLE KEYS */;
/*!40000 ALTER TABLE `puntuacionfecha` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `befInsPuntuacionChk` BEFORE INSERT ON `puntuacionfecha` FOR EACH ROW BEGIN

IF NEW.Puntaje < 1 OR NEW.Puntaje > 10 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La puntuación debe estar entre 1 y 10';
    END IF;
   
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `FechaNacimiento` date NOT NULL,
  `Password` char(64) NOT NULL,
  `EsAdministrador` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Email` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
INSERT INTO `usuario` VALUES (1,'Ruth','Lezcano','ruthlezcano.et12d1@email.com','2008-01-18','5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8',1);
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'GranDT_ET12'
--
/*!50003 DROP FUNCTION IF EXISTS `FN_CalcularGastoPlantilla` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `FN_CalcularGastoPlantilla`(p_PlantillaId INT) RETURNS decimal(12,2)
    READS SQL DATA
    DETERMINISTIC
BEGIN
    DECLARE total DECIMAL(12,2);
   
    SELECT IFNULL(SUM(f.Cotizacion), 0) INTO total
    FROM DetallePlantilla dp
    INNER JOIN Futbolista f ON dp.FutbolistaId = f.Id
    WHERE dp.PlantillaId = p_PlantillaId;
   
    RETURN total;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP FUNCTION IF EXISTS `FN_CalcularPuntuacionPlantillaFecha` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `FN_CalcularPuntuacionPlantillaFecha`(p_PlantillaId INT, p_NumeroFecha INT) RETURNS decimal(6,2)
    READS SQL DATA
    DETERMINISTIC
BEGIN
    DECLARE totalPuntos DECIMAL(6,2);
   
    SELECT IFNULL(SUM(pf.Puntaje), 0) INTO totalPuntos
    FROM DetallePlantilla dp
    INNER JOIN PuntuacionFecha pf ON dp.FutbolistaId = pf.FutbolistaId
    WHERE dp.PlantillaId = p_PlantillaId AND pf.NumeroFecha = p_NumeroFecha;
   
    RETURN totalPuntos;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP FUNCTION IF EXISTS `FN_ContarFutbolistas` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `FN_ContarFutbolistas`() RETURNS int
    READS SQL DATA
    DETERMINISTIC
BEGIN
    DECLARE total INT;
   
    SELECT COUNT(*) INTO total
    FROM Futbolista;
   
    RETURN total;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP FUNCTION IF EXISTS `FN_ExisteEquipo` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `FN_ExisteEquipo`(p_Id INT) RETURNS tinyint(1)
    READS SQL DATA
BEGIN
    DECLARE v_Existe BOOLEAN DEFAULT FALSE;
   
    -- Cambiá 'Equipo' por 'Equipos' si tu tabla está en plural en Workbench
    IF EXISTS (SELECT 1 FROM Equipo WHERE Id = p_Id) THEN
        SET v_Existe = TRUE;
    END IF;
    RETURN v_Existe;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP FUNCTION IF EXISTS `FN_ExisteFutbolista` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `FN_ExisteFutbolista`(p_Id INT) RETURNS tinyint(1)
    READS SQL DATA
BEGIN
    DECLARE v_Existe BOOLEAN DEFAULT FALSE;
   
    IF EXISTS (SELECT 1 FROM Futbolista WHERE Id = p_Id) THEN
        SET v_Existe = TRUE;
    END IF;
    RETURN v_Existe;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `SP_AgregarJugadorPlantilla` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_AgregarJugadorPlantilla`(
    IN p_PlantillaId INT,
    IN p_FutbolistaId INT,
    IN p_EsTitular BOOLEAN
)
BEGIN
    INSERT INTO DetallePlantilla (PlantillaId, FutbolistaId, EsTitular)
    VALUES (p_PlantillaId, p_FutbolistaId, p_EsTitular);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `SP_RegistrarFutbolista` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_RegistrarFutbolista`(
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
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `SP_RegistrarPlantilla` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_RegistrarPlantilla`(
    IN p_UsuarioId INT,
    IN p_PresupuestoMaximo DECIMAL(12, 2),
    IN p_CantidadMaximaJugadores INT
)
BEGIN
    INSERT INTO Plantilla (UsuarioId, PresupuestoMaximo, CantidadMaximaJugadores)
    VALUES (p_UsuarioId, p_PresupuestoMaximo, p_CantidadMaximaJugadores);
   
    SELECT LAST_INSERT_ID();
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `SP_RegistrarPuntuacion` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_RegistrarPuntuacion`(
    IN p_FutbolistaId INT,
    IN p_NumeroFecha INT,
    IN p_Puntaje DECIMAL(4, 2)
)
BEGIN
    INSERT INTO PuntuacionFecha (FutbolistaId, NumeroFecha, Puntaje)
    VALUES (p_FutbolistaId, p_NumeroFecha, p_Puntaje);
   
    SELECT LAST_INSERT_ID();
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `SP_RegistrarUsuario` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_RegistrarUsuario`(
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
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-11-14 23:33:30
