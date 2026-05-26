IF OBJECT_ID('dbo.RelationSharedAlbumPhotoDelete', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RelationSharedAlbumPhotoDelete;
	PRINT 'Procedura RelationSharedAlbumPhotoDelete byla smazána.';
GO
CREATE PROCEDURE dbo.RelationSharedAlbumPhotoDelete
	@PhotoId INT,
    @SharedAlbumId INT
AS
BEGIN
	SET NOCOUNT ON;
    DELETE FROM RelationSharedAlbumPhoto
    WHERE 
        PhotoId = @PhotoId AND
		SharedAlbumId = @SharedAlbumId
END
GO