IF OBJECT_ID('dbo.PhotoSelectBySharedAlbumId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoSelectBySharedAlbumId;
	PRINT 'Procedura PhotoSelectBySharedAlbumId byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoSelectBySharedAlbumId
	@SharedAlbumId INT,
    @Offset INT, -- od jakého záznamu načítám   
    @Fetch INT -- počet řádků, které načítám
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  P.Id
      ,PhotoName
      ,PhotoDescription
      ,OwnerId
	  ,GroupsId
      ,FileSize
      ,FSFileName
      ,Personal
      ,dbo.ConvertUtcToCzechTime(P.CreateDateTime) AS CreateDateTime     
      ,CreateAuthor
  FROM 
	RelationSharedAlbumPhoto AS RSAP
  LEFT JOIN
    Photo AS P ON RSAP.PhotoId = P.Id
  WHERE
	RSAP.SharedAlbumId = @SharedAlbumId
  ORDER BY RSAP.Id DESC
  OFFSET @Offset ROWS
  FETCH NEXT @Fetch ROWS ONLY;
END
GO