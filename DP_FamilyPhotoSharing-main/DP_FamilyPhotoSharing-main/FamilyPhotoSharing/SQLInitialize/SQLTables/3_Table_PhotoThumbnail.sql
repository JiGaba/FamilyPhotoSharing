IF OBJECT_ID('dbo.PhotoThumbnail', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.PhotoThumbnail;
    PRINT 'Tabulka PhotoThumbnail byla smazána.';
END
GO
BEGIN
	CREATE TABLE PhotoThumbnail (
		Id INT PRIMARY KEY IDENTITY(1,1),
		PhotoId INT NOT NULL,
		FileSize INT NOT NULL,
		FSThumbnailName NVARCHAR(255) NOT NULL,
		CreateDateTime DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
		CreateAuthor INT NOT NULL DEFAULT 0
	);
END
GO