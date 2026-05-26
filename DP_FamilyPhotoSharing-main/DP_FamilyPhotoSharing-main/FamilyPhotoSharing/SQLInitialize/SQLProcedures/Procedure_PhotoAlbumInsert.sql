IF OBJECT_ID('dbo.PhotoAlbumInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoAlbumInsert;
	PRINT 'Procedura PhotoAlbumInsert byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoAlbumInsert
	@AlbumName NVARCHAR(255),
    @AlbumDescription NVARCHAR(MAX),
    @Personal BIT,
	@TitlePhotoId INT,
    @UserGroupsId INT,
    @OwnerUserId INT,
	@CreateAuthor INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO PhotoAlbum 
	(
        AlbumName,
        AlbumDescription,
        Personal,
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
        @Personal,
		@TitlePhotoId,
        @UserGroupsId,
        @OwnerUserId,
        @CreateAuthor,
        SYSUTCDATETIME()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO