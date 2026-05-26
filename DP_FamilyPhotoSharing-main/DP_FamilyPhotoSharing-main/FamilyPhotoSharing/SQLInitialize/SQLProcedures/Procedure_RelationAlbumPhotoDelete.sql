IF OBJECT_ID('dbo.RelationAlbumPhotoDelete', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RelationAlbumPhotoDelete;
	PRINT 'Procedura RelationAlbumPhotoDelete byla smazána.';
GO
CREATE PROCEDURE dbo.RelationAlbumPhotoDelete
	@PhotoId INT,
    @GroupId INT,
    @AlbumId INT
AS
BEGIN
	SET NOCOUNT ON;
    DELETE FROM RelationAlbumPhoto 
    WHERE 
        AlbumId = @AlbumId AND
        GroupId = @GroupId AND
		PhotoId = @PhotoId
END
GO