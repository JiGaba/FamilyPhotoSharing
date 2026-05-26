IF OBJECT_ID('dbo.UserGroupsInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserGroupsInsert;
	PRINT 'Procedura UserGroupsInsert byla smazána.';
GO
CREATE PROCEDURE dbo.UserGroupsInsert
	@GroupName NVARCHAR(100),
    @GroupDescription NVARCHAR(MAX),
    @FolderName NVARCHAR(100),
    @CreateAuthor INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO UserGroups 
	(
        GroupName
        ,GroupDescription
        ,FolderName
        ,CreateDateTime
        ,CreateAuthor
    )
    VALUES 
	(
        @GroupName
        ,@GroupDescription
        ,@FolderName
        ,SYSUTCDATETIME()
        ,@CreateAuthor
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT)
END
GO