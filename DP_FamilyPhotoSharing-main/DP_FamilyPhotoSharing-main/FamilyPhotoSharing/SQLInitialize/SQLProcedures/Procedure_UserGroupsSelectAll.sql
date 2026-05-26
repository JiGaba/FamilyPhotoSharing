IF OBJECT_ID('dbo.UserGroupsSelectAll', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserGroupsSelectAll;
	PRINT 'Procedura UserGroupsSelectAll byla smazána.';
GO
CREATE PROCEDURE dbo.UserGroupsSelectAll
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
END
GO