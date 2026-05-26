IF OBJECT_ID('dbo.PhotoSelectByAlbumId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoSelectByAlbumId;
	PRINT 'Procedura PhotoSelectByAlbumId byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoSelectByAlbumId
	@AlbumId INT,
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
	RelationAlbumPhoto AS RAP
  LEFT JOIN
    Photo AS P ON RAP.PhotoId = P.Id
  WHERE
	RAP.AlbumId = @AlbumId
  ORDER BY RAP.Id DESC
  OFFSET @Offset ROWS
  FETCH NEXT @Fetch ROWS ONLY;
END
GO