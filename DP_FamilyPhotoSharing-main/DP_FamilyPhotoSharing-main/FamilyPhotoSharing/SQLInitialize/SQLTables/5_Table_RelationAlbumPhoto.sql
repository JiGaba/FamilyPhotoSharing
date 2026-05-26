IF OBJECT_ID('dbo.RelationAlbumPhoto', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.RelationAlbumPhoto;
    PRINT 'Tabulka RelationAlbumPhoto byla smazána.';
END
GO
BEGIN
	CREATE TABLE RelationAlbumPhoto (
		Id INT PRIMARY KEY IDENTITY(1,1),
		AlbumId INT NOT NULL,
		GroupId INT NOT NULL,
		PhotoId INT NOT NULL,
		CreateDateTime DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
		CreateAuthorId INT NOT NULL DEFAULT 0,
	);
END
GO