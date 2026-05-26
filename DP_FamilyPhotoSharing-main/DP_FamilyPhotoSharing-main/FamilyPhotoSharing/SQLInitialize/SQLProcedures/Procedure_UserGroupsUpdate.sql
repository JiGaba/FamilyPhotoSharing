IF OBJECT_ID('dbo.UserGroupsUpdate', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserGroupsUpdate;
	PRINT 'Procedura UserGroupsUpdate byla smazána.';
GO
CREATE PROCEDURE dbo.UserGroupsUpdate
    @GroupId INT
    ,@GroupName NVARCHAR(100)
    ,@GroupDescription NVARCHAR(MAX)
    ,@CreateAuthor INT
AS
BEGIN
    SET NOCOUNT ON;
	UPDATE UserGroups
    SET
      GroupName = @GroupName
      ,GroupDescription = @GroupDescription
	  ,CreateDateTime = SYSUTCDATETIME()
      ,CreateAuthor = @CreateAuthor
    WHERE
       Id = @GroupId
END
GO