IF OBJECT_ID('dbo.PhotoSelect', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoSelect;
	PRINT 'Procedura PhotoSelect byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoSelect
	@Id INT
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  Id
      ,PhotoName
      ,PhotoDescription
      ,OwnerId
	  ,GroupsId
      ,FileSize
      ,FSFileName
      ,Personal
      ,dbo.ConvertUtcToCzechTime(CreateDateTime) AS CreateDateTime     
      ,CreateAuthor
  FROM 
	Photo
  WHERE
	Id = @Id
END
GO