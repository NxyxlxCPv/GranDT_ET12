
use grandt_et12;
-- =========================================================================
-- 1. INSERCIÓN DE EQUIPOS (Respetando el campo 'Name' definido en el DDL)
-- =========================================================================
INSERT INTO Equipo (Nombre) VALUES
('River Plate'),             -- Id: 1
('Boca Juniors'),            -- Id: 2
('Independiente'),           -- Id: 3
('Racing Club'),             -- Id: 4
('Club Atlético Platense');  -- Id: 5


-- =========================================================================
-- 2. INSERCIÓN DE FUTBOLISTAS (Orden de columnas correcto y tipos válidos)
-- =========================================================================
INSERT INTO Futbolista (Nombre, Apellido, Apodo, FechaNacimiento, Tipo, Cotizacion, EquipoId) 
VALUES

-- ================= RIVER PLATE (EquipoId: 1) =================
('Franco', 'Armani', 'El Pulpo', '1986-10-16', 'Arquero', 15000000.00, 1),
('Jeremías', 'Ledesma', 'Conan', '1993-02-13', 'Arquero', 12000000.00, 1),
('Paulo', 'Díaz', 'Paulo', '1994-08-25', 'Defensor', 25000000.00, 1),
('Germán', 'Pezzella', 'El Bahiense', '1991-06-27', 'Defensor', 20000000.00, 1),
('Marcos', 'Acuña', 'El Huevo', '1991-10-28', 'Defensor', 22000000.00, 1),
('Fabricio', 'Bustos', 'El Tractor', '1996-04-28', 'Defensor', 18000000.00, 1),
('Enzo', 'Díaz', 'Enzo', '1995-09-24', 'Defensor', 16000000.00, 1),
('Enzo', 'Pérez', 'Enzo', '1986-02-22', 'Mediocampista', 14000000.00, 1),
('Rodrigo', 'Aliendro', 'Peti', '1991-02-16', 'Mediocampista', 15000000.00, 1),
('Manuel', 'Lanzini', 'Manu', '1993-02-15', 'Mediocampista', 20000000.00, 1),
('Ignacio', 'Fernández', 'Nacho', '1990-01-12', 'Mediocampista', 18000000.00, 1),
('Claudio', 'Echeverri', 'El Diablito', '2006-01-02', 'Mediocampista', 30000000.00, 1),
('Facundo', 'Colidio', 'Facu', '2000-01-04', 'Delantero', 22000000.00, 1),
('Miguel', 'Borja', 'El Colibrí', '1993-01-26', 'Delantero', 28000000.00, 1),
('Pablo', 'Solari', 'Pablito', '2001-03-22', 'Delantero', 20000000.00, 1),

-- ================= BOCA JUNIORS (EquipoId: 2) =================
('Sergio', 'Romero', 'Chiquito', '1987-02-22', 'Arquero', 14000000.00, 2),
('Leandro', 'Brey', 'Lea', '2002-09-21', 'Arquero', 10000000.00, 2),
('Marcos', 'Rojo', 'Marquitos', '1990-03-20', 'Defensor', 18000000.00, 2),
('Nicolás', 'Figal', 'Nico', '1994-04-03', 'Defensor', 16000000.00, 2),
('Luis', 'Advíncula', 'El Rayo', '1990-03-02', 'Defensor', 17000000.00, 2),
('Lautaro', 'Blanco', 'Lauti', '1999-02-19', 'Defensor', 19000000.00, 2),
('Aaron', 'Anselmino', 'Aaron', '2005-04-29', 'Defensor', 25000000.00, 2),
('Pol', 'Fernández', 'Pol', '1991-10-11', 'Mediocampista', 15000000.00, 2),
('Cristian', 'Medina', 'Maravilla', '2002-06-01', 'Mediocampista', 26000000.00, 2),
('Kevin', 'Zenón', 'Kevin', '2001-07-30', 'Mediocampista', 25000000.00, 2),
('Ignacio', 'Miramón', 'Nacho', '2003-06-12', 'Mediocampista', 16000000.00, 2),
('Edinson', 'Cavani', 'El Matador', '1987-02-14', 'Delantero', 35000000.00, 2),
('Miguel', 'Merentiel', 'La Bestia', '1996-02-24', 'Delantero', 30000000.00, 2),
('Exequiel', 'Zeballos', 'El Changuito', '2002-04-24', 'Delantero', 20000000.00, 2),
('Milton', 'Giménez', 'Milton', '1996-08-12', 'Delantero', 18000000.00, 2),

-- ================= INDEPENDIENTE (EquipoId: 3) =================
('Rodrigo', 'Rey', 'Rey', '1991-03-08', 'Arquero', 16000000.00, 3),
('Diego', 'Segovia', 'Diego', '2000-05-15', 'Arquero', 8000000.00, 3),
('Joaquín', 'Laso', 'Laso', '1990-07-04', 'Defensor', 12000000.00, 3),
('Kevin', 'Lomónaco', 'Kevin', '2002-01-08', 'Defensor', 18000000.00, 3),
('Federico', 'Vera', 'Fede', '1998-03-24', 'Defensor', 14000000.00, 3),
('Damián', 'Pérez', 'Damián', '1988-12-22', 'Defensor', 10000000.00, 3),
('Ivan', 'Marcone', 'El Capo', '1990-06-03', 'Mediocampista', 15000000.00, 3),
('Lucas', 'González', 'Saltita', '2000-06-03', 'Mediocampista', 14000000.00, 3),
('Felipe', 'Loyola', 'Pipe', '2000-11-09', 'Mediocampista', 18000000.00, 3),
('Federico', 'Mancuello', 'Mancu', '1989-03-26', 'Mediocampista', 12000000.00, 3),
('Gabriel', 'Ávalos', 'El Tanque', '1990-10-12', 'Delantero', 20000000.00, 3),
('Santiago', 'Hidalgo', 'Santi', '2005-02-17', 'Delantero', 15000000.00, 3),
('Alexis', 'Canelo', 'Canelo', '1992-02-03', 'Delantero', 14000000.00, 3),
('Matías', 'Giménez', 'Mati', '1999-03-06', 'Delantero', 16000000.00, 3),

-- ================= RACING CLUB (EquipoId: 4) =================
('Gabriel', 'Arias', 'Gabi', '1987-09-13', 'Arquero', 15000000.00, 4),
('Facundo', 'Cambeses', 'Facu', '1997-04-09', 'Arquero', 11000000.00, 4),
('Leonardo', 'Sigali', 'El Oso', '1987-05-29', 'Defensor', 12000000.00, 4),
('Agustín', 'García Basso', 'Agus', '1992-04-25', 'Defensor', 17000000.00, 4),
('Marco', 'Di Cesare', 'Marco', '2002-01-30', 'Defensor', 20000000.00, 4),
('Gastón', 'Martirena', 'Gasti', '2000-01-05', 'Defensor', 18000000.00, 4),
('Gabriel', 'Rojas', 'Gabi', '1997-06-22', 'Defensor', 16000000.00, 4),
('Bruno', 'Zuculini', 'Zucu', '1993-04-02', 'Mediocampista', 15000000.00, 4),
('Juan Fernando', 'Quintero', 'Juanfer', '1993-01-18', 'Mediocampista', 25000000.00, 4),
('Agustín', 'Almendra', 'Agus', '2000-02-11', 'Mediocampista', 18000000.00, 4),
('Santiago', 'Sosa', 'Santi', '1999-05-03', 'Mediocampista', 22000000.00, 4),
('Adrian', 'Martínez', 'Maravilla', '1992-07-07', 'Delantero', 28000000.00, 4),
('Roger', 'Martínez', 'Roger', '1994-06-23', 'Delantero', 24000000.00, 4),
('Luciano', 'Vietto', 'Lucho', '1993-12-05', 'Delantero', 20000000.00, 4),
('Johan', 'Carbonero', 'Johan', '1999-07-20', 'Delantero', 19000000.00, 4),

-- ================= PLATENSE (EquipoId: 5) =================
('Juan Pablo', 'Cozzani', 'Juampi', '1998-05-09', 'Arquero', 12000000.00, 5),
('Joaquín', 'Blázquez', 'Joaco', '2001-01-28', 'Arquero', 9000000.00, 5),
('Gastón', 'Suso', 'Gaston', '1991-03-12', 'Defensor', 13000000.00, 5),
('Ignacio', 'Vázquez', 'Nacho', '1997-06-15', 'Defensor', 14000000.00, 5),
('Sasha', 'Marcich', 'Sasha', '1998-05-29', 'Defensor', 12000000.00, 5),
('Bautista', 'Barros Schelotto', 'Bauti', '2000-01-13', 'Defensor', 13000000.00, 5),
('Carlos', 'Villalba', 'Carlitos', '1998-07-19', 'Mediocampista', 14000000.00, 5),
('Fernando', 'Juárez', 'Ajito', '1998-08-23', 'Mediocampista', 12000000.00, 5),
('Leonel', 'Picco', 'Leo', '1998-10-22', 'Mediocampista', 15000000.00, 5),
('Franco', 'Mineria', 'Franco', '2002-11-04', 'Mediocampista', 11000000.00, 5),
('Mateo', 'Pellegrino', 'Flaco', '2001-10-22', 'Delantero', 18000000.00, 5),
('Ronaldo', 'Martínez', 'Roni', '1996-04-25', 'Delantero', 15000000.00, 5),
('Augusto', 'Lotti', 'Lotti', '1996-06-10', 'Delantero', 16000000.00, 5),
('Ignacio', 'Schor', 'El Pájaro', '2000-08-04', 'Delantero', 14000000.00, 5);


-- =========================================================================
-- 3. INSERCIÓN DE USUARIOS DE PRUEBA
-- =========================================================================
INSERT INTO Usuario (Nombre, Apellido, Email, FechaNacimiento, Password, EsAdministrador)
VALUES ('Ruth', 'Lezcano', 'ruthlezcano.et12d1@email.com', '2008-01-18', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', TRUE);