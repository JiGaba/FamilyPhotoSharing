IF OBJECT_ID('dbo.RelationUserSharedAlbum', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.RelationUserSharedAlbum;
    PRINT 'Tabulka RelationUserSharedAlbum byla smazána.';
END
GO
BEGIN
	CREATE TABLE RelationUserSharedAlbum (
		Id INT PRIMARY KEY IDENTITY(1,1),
		UserId INT NOT NULL,
		SharedAlbumId INT NOT NULL,
		CreateDateTime DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
		CreateAuthorId INT NOT NULL DEFAULT 0,
	);
END
GO