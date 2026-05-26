IF OBJECT_ID('dbo.SystemLog', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.SystemLog;
    PRINT 'Tabulka SystemLog byla smazána.';
END
GO
BEGIN
	CREATE TABLE SystemLog (
		Id INT PRIMARY KEY IDENTITY(1,1),
		LogType INT NOT NULL,
		ActionType INT NOT NULL,
		LogDescription NVARCHAR(MAX) NOT NULL DEFAULT '',
		CreateAuthorId INT NOT NULL,
		GroupsId INT NOT NULL,
		CreateDate DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()
	);
END
GO