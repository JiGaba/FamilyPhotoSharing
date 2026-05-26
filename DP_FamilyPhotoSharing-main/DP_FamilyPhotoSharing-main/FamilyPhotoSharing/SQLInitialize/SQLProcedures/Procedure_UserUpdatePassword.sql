IF OBJECT_ID('dbo.UserUpdatePassword', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserUpdatePassword;
	PRINT 'Procedura UserUpdatePassword byla smazána.';
GO
CREATE PROCEDURE dbo.UserUpdatePassword
	@Id INT,
	@UserPassword NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET
		UserPassword = @UserPassword,
        CreateDateTime = SYSUTCDATETIME()
    WHERE
        Id = @Id;
END
GO