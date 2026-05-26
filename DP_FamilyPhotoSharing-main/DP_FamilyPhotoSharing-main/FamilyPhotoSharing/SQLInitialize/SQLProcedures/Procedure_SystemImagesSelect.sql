IF OBJECT_ID('dbo.SystemImagesSelect', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SystemImagesSelect;
	PRINT 'Procedura SystemImagesSelect byla smazána.';
GO
CREATE PROCEDURE dbo.SystemImagesSelect
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  Id
      ,Size
	  ,FSPhotoName
	  ,PhotoNameOriginal
	  ,dbo.ConvertUtcToCzechTime(CreateDateTime) AS CreateDateTime
	  ,CreateAuthorId
    FROM 
	   SystemImages
    WHERE
       Id = @Id
END
GO