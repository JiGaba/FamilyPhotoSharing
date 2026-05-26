IF OBJECT_ID('dbo.RelationUserSharedAlbumDelete', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RelationUserSharedAlbumDelete;
	PRINT 'Procedura RelationUserSharedAlbumDelete byla smazána.';
GO
CREATE PROCEDURE dbo.RelationUserSharedAlbumDelete
	@UserId INT,
    @SharedAlbumId INT
AS
BEGIN
	SET NOCOUNT ON;
    DELETE FROM RelationUserSharedAlbum 
    WHERE 
        UserId = @UserId AND
		SharedAlbumId = @SharedAlbumId
END
GO