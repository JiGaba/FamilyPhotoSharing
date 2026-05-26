IF OBJECT_ID('dbo.PhotoThumbnailSelectByPhotoId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoThumbnailSelectByPhotoId;
	PRINT 'Procedura PhotoThumbnailSelectByPhotoId byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoThumbnailSelectByPhotoId
	@PhotoId INT
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  Id
      ,PhotoId
      ,FileSize
      ,FSThumbnailName
      ,dbo.ConvertUtcToCzechTime(CreateDateTime) AS CreateDateTime     
      ,CreateAuthor
  FROM 
	PhotoThumbnail
  WHERE
	PhotoId = @PhotoId
END
GO
