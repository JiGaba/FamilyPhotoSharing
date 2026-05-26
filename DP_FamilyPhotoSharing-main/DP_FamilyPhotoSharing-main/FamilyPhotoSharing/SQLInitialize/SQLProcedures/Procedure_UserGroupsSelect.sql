IF OBJECT_ID('dbo.UserGroupsSelect', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserGroupsSelect;
	PRINT 'Procedura UserGroupsSelect byla smazána.';
GO
CREATE PROCEDURE dbo.UserGroupsSelect
    @GroupId INT
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  Id
      ,GroupName
      ,GroupDescription
      ,FolderName
	  ,dbo.ConvertUtcToCzechTime(CreateDateTime) AS CreateDateTime
      ,CreateAuthor
    FROM 
	   UserGroups
    WHERE
       Id = @GroupId
END
GO