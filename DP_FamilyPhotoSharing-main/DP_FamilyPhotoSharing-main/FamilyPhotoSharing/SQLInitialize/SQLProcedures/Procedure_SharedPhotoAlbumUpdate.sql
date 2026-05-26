IF OBJECT_ID('dbo.SharedPhotoAlbumUpdate', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SharedPhotoAlbumUpdate;
	PRINT 'Procedura SharedPhotoAlbumUpdate byla smazána.';
GO
CREATE PROCEDURE dbo.SharedPhotoAlbumUpdate
    @Id INT,
	@AlbumName NVARCHAR(255),
    @AlbumDescription NVARCHAR(MAX),
	@TitlePhotoId NVARCHAR(255),
    @UserGroupsId INT,
    @OwnerUserId INT,
	@CreateAuthor INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE SharedPhotoAlbum SET 
        AlbumName = @AlbumName,
        AlbumDescription = @AlbumDescription,
		TitlePhotoId = @TitlePhotoId,
        UserGroupsId = @UserGroupsId,
        OwnerUserId = @OwnerUserId,
        CreateAuthor = @CreateAuthor,
        CreateDateTime = SYSUTCDATETIME()
    WHERE
        Id = @Id;
END
GO