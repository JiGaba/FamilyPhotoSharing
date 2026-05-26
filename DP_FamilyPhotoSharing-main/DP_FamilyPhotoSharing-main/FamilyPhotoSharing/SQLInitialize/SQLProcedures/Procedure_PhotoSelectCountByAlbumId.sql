IF OBJECT_ID('dbo.PhotoSelectCountByAlbumId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoSelectCountByAlbumId;
	PRINT 'Procedura PhotoSelectCountByAlbumId byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoSelectCountByAlbumId
    @AlbumId INT
AS
BEGIN
  SET NOCOUNT ON;
  SELECT
	COUNT(*) AS TotalCount
  FROM 
	RelationAlbumPhoto
  WHERE
	AlbumId = @AlbumId
END
GO