CREATE DATABASE IF NOT EXISTS GranDT_ET12;
USE GranDT_ET12;

-- 1. Tabla de Usuarios (Soporta hasta 2000 usuarios, password fija de 64 caracteres)
CREATE TABLE Usuario (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    FechaNacimiento DATE NOT NULL,
    Password CHAR(64) NOT NULL, -- Longitud fija de 64 caracteres para la contraseña encriptada
    EsAdministrador BOOLEAN NOT NULL DEFAULT FALSE
);

-- 2. Tabla de Equipos (Máximo 32 equipos, nombres únicos)
CREATE TABLE Equipo (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL UNIQUE
);

-- 3. Tabla de Futbolistas (Máximo 1500 jugadores, cotización hasta 99.999.999,99)
CREATE TABLE Futbolista (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Apodo VARCHAR(50) NULL,
    FechaNacimiento DATE NOT NULL,
    Tipo ENUM('Arquero', 'Defensor', 'Mediocampista', 'Delantero') NOT NULL,
    Cotizacion DECIMAL(10, 2) NOT NULL,
    EquipoId INT NOT NULL,
    CONSTRAINT FK_Futbolista_Equipo FOREIGN KEY (EquipoId) REFERENCES Equipo(Id)
); 

-- 4. Tabla de Plantillas (Creadas por los usuarios)
CREATE TABLE Plantilla (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    PresupuestoMaximo DECIMAL(12, 2) NOT NULL,
    CantidadMaximaJugadores INT NOT NULL DEFAULT 20,
    CONSTRAINT FK_Plantilla_Usuario FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
);

-- 5. Tabla Intermedia DetallePlantilla (Relación Muchos a Muchos entre Plantilla y Futbolista)
CREATE TABLE DetallePlantilla (
    PlantillaId INT NOT NULL,
    FutbolistaId INT NOT NULL,
    EsTitular BOOLEAN NOT NULL, -- True para titular, False para suplente
    PRIMARY KEY (PlantillaId, FutbolistaId),
    CONSTRAINT FK_DetallePlantilla_Plantilla FOREIGN KEY (PlantillaId) REFERENCES Plantilla(Id),
    CONSTRAINT FK_DetallePlantilla_Futbolista FOREIGN KEY (FutbolistaId) REFERENCES Futbolista(Id)
); 

-- 6. Tabla de Puntuaciones (Fecha menor a 50, nota decimal de 1 a 10, única por jugador y fecha)
CREATE TABLE PuntuacionFecha (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FutbolistaId INT NOT NULL,
    NumeroFecha INT NOT NULL,
    Puntaje DECIMAL(4, 2) NOT NULL, -- Rango decimal de 1 a 10
    CONSTRAINT CHK_NumeroFecha CHECK (NumeroFecha < 50 AND NumeroFecha > 0),
    CONSTRAINT CHK_Puntaje CHECK (Puntaje >= 1.00 AND Puntaje <= 10.00),
    CONSTRAINT UQ_Futbolista_Fecha UNIQUE (FutbolistaId, NumeroFecha),
    CONSTRAINT FK_PuntuacionFecha_Futbolista FOREIGN KEY (FutbolistaId) REFERENCES Futbolista(Id)
); 
