IF OBJECT_ID('dbo.PhotoSelectCountByOwnerAndPersonal', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoSelectCountByOwnerAndPersonal;
	PRINT 'Procedura PhotoSelectCountByOwnerAndPersonal byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoSelectCountByOwnerAndPersonal
	@OwnerId INT,
    @Personal BIT
AS
BEGIN
  SET NOCOUNT ON;
  SELECT
	COUNT(*) AS TotalCount
  FROM 
	Photo AS P
    LEFT JOIN RelationAlbumPhoto AS RAP ON RAP.PhotoId = P.Id 
  WHERE
	OwnerId = @OwnerId
    AND
    Personal = @Personal
    AND
    (@Personal = 0 OR RAP.PhotoId IS NULL)
END
GO