IF OBJECT_ID('dbo.SharedPhotoAlbumInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SharedPhotoAlbumInsert;
	PRINT 'Procedura SharedPhotoAlbumInsert byla smazána.';
GO
CREATE PROCEDURE dbo.SharedPhotoAlbumInsert
	@AlbumName NVARCHAR(255),
    @AlbumDescription NVARCHAR(MAX),
	@TitlePhotoId INT,
    @UserGroupsId INT,
    @OwnerUserId INT,
	@CreateAuthor INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO SharedPhotoALbum
	(
        AlbumName,
        AlbumDescription,
		TitlePhotoId,
        UserGroupsId,
        OwnerUserId,
        CreateAuthor,
        CreateDateTime
    )
    VALUES 
	(
        @AlbumName,
        @AlbumDescription,
		@TitlePhotoId,
        @UserGroupsId,
        @OwnerUserId,
        @CreateAuthor,
        SYSUTCDATETIME()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO