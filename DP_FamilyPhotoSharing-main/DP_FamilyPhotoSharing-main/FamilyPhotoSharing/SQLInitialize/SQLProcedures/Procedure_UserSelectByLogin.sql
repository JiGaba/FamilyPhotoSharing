IF OBJECT_ID('dbo.UserSelectByLogin', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserSelectByLogin;
	PRINT 'Procedura UserSelectByLogin byla smazána.';
GO
CREATE PROCEDURE dbo.UserSelectByLogin
	@UserLogin NVARCHAR(100)
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
	UserLogin = @UserLogin
END
GO