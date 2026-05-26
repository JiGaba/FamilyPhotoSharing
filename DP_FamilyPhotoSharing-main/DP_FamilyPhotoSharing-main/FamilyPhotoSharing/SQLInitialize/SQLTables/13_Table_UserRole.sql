IF OBJECT_ID('dbo.UserRoles', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.UserRoles;
    PRINT 'Tabulka UserRoles byla smazána.';
END
GO

CREATE TABLE UserRoles(
	Id INT PRIMARY KEY IDENTITY(1,1),
	RoleAlias NVARCHAR(20) NOT NULL,
	RoleName NVARCHAR(50) NOT NULL,
	RoleDescription NVARCHAR(MAX) NOT NULL
);
PRINT 'Tabulka UserRoles byla vytvořena.';
GO

INSERT INTO UserRoles (RoleAlias, RoleName, RoleDescription) 
VALUES 
	('Admin', 'Administrátor','Administrátor celého systému. Může vytvářet a editovat uživatele a uživatelské skupiny skupiny. Je mu umožněno vytvářet a editovat veřejné a vlastní galerie. Může editovat veřejné fotografie, dále přidávat, editovat a sdílet vlastní fotografie. Vytvářet alba a sdílet vlastní fotografie.'),
	('GroupAdmin', 'Administrátor skupiny','Administrátor dané skupiny. Může vytvářet a editovat uživatele v rámci skupiny. Je mu umožněno vytvářet a editovat veřejné a vlastní galerie. Může editovat veřejné fotografie, dále přidávat, editovat a sdílet vlastní fotografie. Vytvářet alba a sdílet vlastní fotografie.'),
	('User', 'Uživatel','Může přidávat, editovat a sdílet vlastní fotografie. Vytvářet alba a sdílet vlastní fotografie.'),
	('Host', 'Host','Může prohlížet přiřazené fotografie.')
PRINT 'Tabulka UserRoles byla naplněna daty.';
GO