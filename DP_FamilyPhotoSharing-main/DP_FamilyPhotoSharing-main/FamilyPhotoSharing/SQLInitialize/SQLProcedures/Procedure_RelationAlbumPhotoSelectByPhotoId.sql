IF OBJECT_ID('dbo.RelationAlbumPhotoSelectByPhotoId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RelationAlbumPhotoSelectByPhotoId;
	PRINT 'Procedura RelationAlbumPhotoSelectByPhotoId byla smazána.';
GO
CREATE PROCEDURE dbo.RelationAlbumPhotoSelectByPhotoId
	@PhotoId INT
AS
BEGIN
	SET NOCOUNT ON;
    SELECT
	    Id,
        AlbumId,
        GroupId,
		PhotoId,
        CreateAuthorId,
        CreateDateTime
    FROM
        RelationAlbumPhoto
    Where PhotoId = @PhotoId
END
GO