IF OBJECT_ID('dbo.UserSelect', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserSelect;
	PRINT 'Procedura UserSelect byla smazána.';
GO
CREATE PROCEDURE dbo.UserSelect
	@UserId INT
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
	Id = @UserId
END
GO