IF OBJECT_ID('dbo.PhotoSelectByOwnerIdAndPersonal', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoSelectByOwnerIdAndPersonal;
	PRINT 'Procedura PhotoSelectByOwnerIdAndPersonal byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoSelectByOwnerIdAndPersonal
	@OwnerId INT,
    @Personal BIT,
    @Offset INT, -- od jakého záznamu naèítám   
    @Fetch INT -- poèet øádkù, které naèítám
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
	OwnerId = @OwnerId
    AND
    Personal = @Personal
    AND
    (@Personal = 0 OR RAP.PhotoId IS NULL)
  ORDER BY Id DESC
  OFFSET @Offset ROWS
  FETCH NEXT @Fetch ROWS ONLY;
END
GO