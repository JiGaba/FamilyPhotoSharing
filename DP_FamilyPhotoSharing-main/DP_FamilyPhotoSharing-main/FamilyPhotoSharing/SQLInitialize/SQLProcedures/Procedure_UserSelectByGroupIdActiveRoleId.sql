IF OBJECT_ID('dbo.UserSelectByGroupIdActiveRoleId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserSelectByGroupIdActiveRoleId;
	PRINT 'Procedura UserSelectByGroupIdActiveRoleId byla smazána.';
GO
CREATE PROCEDURE dbo.UserSelectByGroupIdActiveRoleId
	@GroupId INT,
    @Active BIT,
    @RoleIds VARCHAR(255),
    @Activated BIT = 1
AS
BEGIN
  SET NOCOUNT ON;
	SELECT
	  Id
      ,RoleId
      ,GroupId
      ,UserLogin
      ,UserName
      ,UserSurname
      ,UserPassword
      ,UserDescription
      ,SystemImagesId
      ,dbo.ConvertUtcToCzechTime(CreateDateTime) AS CreateDateTime
      ,CreateAuthor
      ,Active
      ,Activated
      ,BackupKey
  FROM 
	Users
  WHERE
	GroupId = @GroupId
	AND
	Active = @Active
	AND
    Activated = @Activated
    AND
	RoleId IN (SELECT value FROM STRING_SPLIT(@RoleIds, ','))
END
GO