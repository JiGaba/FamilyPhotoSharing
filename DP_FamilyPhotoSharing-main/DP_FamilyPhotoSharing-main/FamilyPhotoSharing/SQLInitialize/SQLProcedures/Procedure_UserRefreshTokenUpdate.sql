IF OBJECT_ID('dbo.UserRefreshTokenUpdate', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserRefreshTokenUpdate;
	PRINT 'Procedura UserRefreshTokenUpdate byla smazána.';
GO
CREATE PROCEDURE dbo.UserRefreshTokenUpdate
	@UserId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE 
		UserRefreshToken 
	SET 
		IsRevoked = 1
	WHERE 
		UserId = @UserId AND IsRevoked = 0
END
GO