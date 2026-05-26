IF OBJECT_ID('dbo.RelationSharedAlbumPhotoSelectByPhotoId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RelationSharedAlbumPhotoSelectByPhotoId;
	PRINT 'Procedura RelationSharedAlbumPhotoSelectByPhotoId byla smazána.';
GO
CREATE PROCEDURE dbo.RelationSharedAlbumPhotoSelectByPhotoId
    @PhotoId INT
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  Id,
      SharedAlbumId,
      PhotoId,
      CreateDateTime,
      CreateAuthorId
    FROM 
	   RelationSharedAlbumPhoto
    WHERE
       PhotoId = @PhotoId
END
GO