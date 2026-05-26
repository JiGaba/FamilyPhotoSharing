IF OBJECT_ID('dbo.UserSelectAll', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserSelectAll;
	PRINT 'Procedura UserSelectAll byla smazána.';
GO
CREATE PROCEDURE dbo.UserSelectAll
    @GroupId INT = NULL,
    @Filter NVARCHAR(10) = NULL
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
  WHERE (@Filter IS NULL OR GroupId = @GroupId)
END
GO