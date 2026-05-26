IF OBJECT_ID('dbo.RelationSharedAlbumPhoto', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.RelationSharedAlbumPhoto;
    PRINT 'Tabulka RelationSharedAlbumPhoto byla smazána.';
END
GO
BEGIN
	CREATE TABLE RelationSharedAlbumPhoto (
		Id INT PRIMARY KEY IDENTITY(1,1),
		SharedAlbumId INT NOT NULL,
		PhotoId INT NOT NULL,
		CreateDateTime DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
		CreateAuthorId INT NOT NULL DEFAULT 0,
	);
END
GO