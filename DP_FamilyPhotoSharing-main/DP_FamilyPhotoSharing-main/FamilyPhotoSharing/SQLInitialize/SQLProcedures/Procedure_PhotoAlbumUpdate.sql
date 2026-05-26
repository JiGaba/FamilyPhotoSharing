IF OBJECT_ID('dbo.PhotoAlbumUpdate', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoAlbumUpdate;
	PRINT 'Procedura PhotoAlbumUpdate byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoAlbumUpdate
    @Id INT,
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
	UPDATE PhotoAlbum SET 
        AlbumName = @AlbumName,
        AlbumDescription = @AlbumDescription,
        Personal = @Personal,
		TitlePhotoId = @TitlePhotoId,
        UserGroupsId = @UserGroupsId,
        OwnerUserId = @OwnerUserId,
        CreateAuthor = @CreateAuthor,
        CreateDateTime = SYSUTCDATETIME()
    WHERE
        Id = @Id;
END
GO