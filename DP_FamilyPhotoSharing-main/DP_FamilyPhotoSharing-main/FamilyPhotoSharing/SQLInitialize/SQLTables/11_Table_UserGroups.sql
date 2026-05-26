IF OBJECT_ID('dbo.UserGroups', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.UserGroups;
    PRINT 'Tabulka UserGroups byla smazána.';
END
GO
BEGIN
	CREATE TABLE UserGroups(
		Id INT PRIMARY KEY IDENTITY(1,1),
		GroupName NVARCHAR(100) NOT NULL UNIQUE,
		GroupDescription NVARCHAR(MAX) NOT NULL,
		FolderName NVARCHAR(50) NOT NULL UNIQUE,
		CreateDateTime DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
		CreateAuthor INT NOT NULL DEFAULT 0,
	);
    PRINT 'Tabulka UserGroups byla vytvořena.';

	INSERT INTO UserGroups (GroupName, GroupDescription, FolderName) VALUES
		('Hlavní skupina', 'Automaticky vygenerovaná skupina','main_group')
END
GO