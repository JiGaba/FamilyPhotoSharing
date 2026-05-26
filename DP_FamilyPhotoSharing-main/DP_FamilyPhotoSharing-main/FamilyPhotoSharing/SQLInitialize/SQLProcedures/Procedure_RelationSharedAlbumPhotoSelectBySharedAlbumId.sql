IF OBJECT_ID('dbo.RelationSharedAlbumPhotoSelectBySharedAlbumId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RelationSharedAlbumPhotoSelectBySharedAlbumId;
	PRINT 'Procedura RelationSharedAlbumPhotoSelectBySharedAlbumId byla smazána.';
GO
CREATE PROCEDURE dbo.RelationSharedAlbumPhotoSelectBySharedAlbumId
    @SharedAlbumId INT
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
       SharedAlbumId = @SharedAlbumId
END
GO