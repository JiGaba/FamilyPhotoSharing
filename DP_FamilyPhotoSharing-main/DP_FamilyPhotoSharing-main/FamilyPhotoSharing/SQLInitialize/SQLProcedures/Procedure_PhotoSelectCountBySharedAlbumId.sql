IF OBJECT_ID('dbo.PhotoSelectCountBySharedAlbumId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoSelectCountBySharedAlbumId;
	PRINT 'Procedura PhotoSelectCountBySharedAlbumId byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoSelectCountBySharedAlbumId
    @SharedAlbumId INT
AS
BEGIN
  SET NOCOUNT ON;
  SELECT
	COUNT(*) AS TotalCount
  FROM 
	RelationSharedAlbumPhoto
  WHERE
	SharedAlbumId = @SharedAlbumId
END
GO