IF OBJECT_ID('dbo.UserInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserInsert;
	PRINT 'Procedura UserInsert byla smazána.';
GO
CREATE PROCEDURE dbo.UserInsert
	@UserLogin NVARCHAR(100),
    @UserName NVARCHAR(100),
    @UserSurname NVARCHAR(255),
	@UserPassword NVARCHAR(200),
    @UserDescription NVARCHAR(MAX),
    @RoleId INT,
    @GroupId INT,
    @BackupKey NVARCHAR(200),
    @Activated BIT,
    @SystemImagesId INT,
	@CreateAuthor INT,
    @Active BIT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO Users 
	(
        UserLogin,
        UserName,
        UserSurname,
		UserPassword,
        UserDescription,
        RoleId,
        GroupId,
        SystemImagesId,
        CreateDateTime,
        CreateAuthor,
        Active,
        Activated,
        BackupKey
    )
    VALUES 
	(
        @UserLogin,
        @UserName,
        @UserSurname,
		@UserPassword,
        @UserDescription,
        @RoleId,
        @GroupId,
        @SystemImagesId,
        SYSUTCDATETIME(),    
        @CreateAuthor,         
        @Active,
        @Activated,
        @BackupKey
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT)
END
GO