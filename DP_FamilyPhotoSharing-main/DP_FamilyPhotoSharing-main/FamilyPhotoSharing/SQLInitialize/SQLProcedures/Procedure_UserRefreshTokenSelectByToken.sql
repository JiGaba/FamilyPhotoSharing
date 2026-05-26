IF OBJECT_ID('dbo.UserRefreshTokenSelectByToken', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserRefreshTokenSelectByToken;
	PRINT 'Procedura UserRefreshTokenSelectByToken byla smazána.';
GO
CREATE PROCEDURE dbo.UserRefreshTokenSelectByToken
	@Token NVARCHAR(2000)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		Id,
		UserId,
		Token,
		IsRevoked,
		Expires,
		CreateDateTime,
		CreateAuthorId 
	FROM 
		UserRefreshToken
	WHERE 
		Token = @Token AND IsRevoked = 0
END
GO