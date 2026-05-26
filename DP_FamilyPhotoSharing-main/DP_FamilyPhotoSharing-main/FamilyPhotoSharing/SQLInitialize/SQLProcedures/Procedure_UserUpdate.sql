IF OBJECT_ID('dbo.UserUpdate', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserUpdate;
	PRINT 'Procedura UserUpdate byla smazána.';
GO
CREATE PROCEDURE dbo.UserUpdate
	@Id INT,
    @UserLogin NVARCHAR(100),
	@UserName NVARCHAR(100),
	@UserSurname NVARCHAR(255),
	@UserPassword NVARCHAR(200),
	@UserDescription NVARCHAR(MAX),
	@RoleId INT,
	@SystemImagesId INT,
	@CreateAuthor INT,
	@Active BIT,
    @PasswordChange BIT,
    @Activated BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF @PasswordChange = 1
    BEGIN
        UPDATE Users
        SET
            UserLogin = @UserLogin,
            UserName = @UserName,
            UserSurname = @UserSurname,
		    UserPassword = @UserPassword,
            UserDescription = @UserDescription,
            RoleId = @RoleId,
            SystemImagesId = @SystemImagesId,
            CreateDateTime = SYSUTCDATETIME(),
            CreateAuthor = @CreateAuthor,
            Active = @Active,
            Activated = @Activated
        WHERE
            Id = @Id;
    END
    ELSE
    BEGIN
        UPDATE Users
        SET
            UserLogin = @UserLogin,
            UserName = @UserName,
            UserSurname = @UserSurname,
            UserDescription = @UserDescription,
            RoleId = @RoleId,
            SystemImagesId = @SystemImagesId,
            CreateDateTime = SYSUTCDATETIME(),
            CreateAuthor = @CreateAuthor,
            Active = @Active,
            Activated = @Activated
        WHERE
            Id = @Id;
    END
END
GO