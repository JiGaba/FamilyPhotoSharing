IF OBJECT_ID('dbo.RelationUserSharedAlbumSelectByAlbumId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RelationUserSharedAlbumSelectByAlbumId;
	PRINT 'Procedura RelationUserSharedAlbumSelectByAlbumId byla smazána.';
GO
CREATE PROCEDURE dbo.RelationUserSharedAlbumSelectByAlbumId
    @SharedAlbumId INT
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  Id,
      SharedAlbumId,
      UserId,
      CreateDateTime,
      CreateAuthorId
    FROM 
	   RelationUserSharedAlbum
    WHERE
       SharedAlbumId = @SharedAlbumId
END
GO