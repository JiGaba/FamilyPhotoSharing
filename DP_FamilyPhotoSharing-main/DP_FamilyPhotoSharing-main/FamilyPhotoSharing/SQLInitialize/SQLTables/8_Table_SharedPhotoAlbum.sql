IF OBJECT_ID('dbo.SharedPhotoAlbum', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.SharedPhotoAlbum;
    PRINT 'Tabulka SharedPhotoAlbum byla smazána.';
END
GO
BEGIN
	CREATE TABLE SharedPhotoAlbum (
		Id INT PRIMARY KEY IDENTITY(1,1),
		AlbumName NVARCHAR(255) NOT NULL,
		AlbumDescription NVARCHAR(MAX) NOT NULL DEFAULT '',
		TitlePhotoId INT NOT NULL DEFAULT 0,
		CreateDateTime DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
		CreateAuthor INT NOT NULL DEFAULT 0,
		OwnerUserId INT NOT NULL,
		UserGroupsId INT NOT NULL
	);
END
GO