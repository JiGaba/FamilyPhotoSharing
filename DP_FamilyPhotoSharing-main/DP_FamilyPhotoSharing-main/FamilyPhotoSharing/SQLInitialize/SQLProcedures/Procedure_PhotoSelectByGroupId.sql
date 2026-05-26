IF OBJECT_ID('dbo.PhotoSelectByGroupId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoSelectByGroupId;
	PRINT 'Procedura PhotoSelectByGroupId byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoSelectByGroupId
	@GroupId INT,
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
	Photo AS P
LEFT JOIN RelationAlbumPhoto AS RAP ON RAP.PhotoId = P.Id 
WHERE
	GroupsId = @GroupId
  AND
  P.Personal = 0
  AND
  RAP.PhotoId IS NULL
  ORDER BY Id DESC
  OFFSET @Offset ROWS
  FETCH NEXT @Fetch ROWS ONLY;
END
GO