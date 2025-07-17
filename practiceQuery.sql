USE tilearchive;
INSERT IGNORE INTO glazetype (Name, Description) VALUES 
('Glossy', 'Shiny and Smooth'),
('Matte', 'Not Shiny, Dull');
INSERT IGNORE INTO surfacecondition (NAME) VALUES
('Rough'),
('Smooth');

INSERT IGNORE INTO tileboard (ID, Image, TileCount, DESCRIPTION, CreatedBy)
VALUES (1, 'R2下絵 (1)ＯＦ.JPG', 43, 'Test board for tile samples', 'Tyler');

INSERT IGNORE INTO testpiece (
  BoardID, Image, Color_L, Color_A, Color_B, GlazeTypeID,
  FiringTemperature, ChemicalComposition, SurfaceConditionID,
  EntryDate, FiringType, SoilType
)
VALUES
(1, 'tile1.png', 52.3, 15.2, 12.1, 1, 1230, 'Fe2O3', 1, '2024-07-10', 'OF', 'siratuti'),
(1, 'tile2.png', 63.8, 10.9, 20.0, 2, 1250, 'Co2O3', 2, '2024-07-10', 'OF', 'ziki'),
(1, 'tile3.png', 52.3, 15.2, 12.1, 1, 1230, 'Fe2O3', 1, '2024-07-10', 'OF', 'siratuti'),
(1, 'tile4.png', 63.8, 10.9, 20.0, 2, 1250, 'Co2O3', 2, '2024-07-10', 'OF', 'ziki'),
(1, 'tile5.png', 52.3, 15.2, 12.1, 1, 1230, 'Fe2O3', 1, '2024-07-10', 'OF', 'siratuti'),
(1, 'tile6.png', 63.8, 10.9, 20.0, 2, 1250, 'Co2O3', 2, '2024-07-10', 'OF', 'ziki'),
(1, 'tile7.png', 52.3, 15.2, 12.1, 1, 1230, 'Fe2O3', 1, '2024-07-10', 'OF', 'siratuti'),
(1, 'tile8.png', 63.8, 10.9, 20.0, 2, 1250, 'Co2O3', 2, '2024-07-10', 'OF', 'ziki'),
(1, 'tile9.png', 52.3, 15.2, 12.1, 1, 1230, 'Fe2O3', 1, '2024-07-10', 'OF', 'siratuti'),
(1, 'tile10.png', 63.8, 10.9, 20.0, 2, 1250, 'Co2O3', 2, '2024-07-10', 'OF', 'ziki');