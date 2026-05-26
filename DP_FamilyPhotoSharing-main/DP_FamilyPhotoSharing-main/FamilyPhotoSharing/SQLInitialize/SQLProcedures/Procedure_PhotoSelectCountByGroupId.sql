IF OBJECT_ID('dbo.PhotoSelectCountByGroupId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoSelectCountByGroupId;
	PRINT 'Procedura PhotoSelectCountByGroupId byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoSelectCountByGroupId
    @GroupId INT
AS
BEGIN
  SET NOCOUNT ON;
  SELECT
	COUNT(*) AS TotalCount
  FROM 
	Photo AS P
  LEFT JOIN RelationAlbumPhoto AS RAP ON RAP.PhotoId = P.Id 
  WHERE
	GroupsId = @GroupId
    AND
    P.Personal = 0
    AND
    RAP.PhotoId IS NULL
END
GO