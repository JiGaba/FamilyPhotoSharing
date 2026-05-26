IF OBJECT_ID('dbo.UserRefreshTokenInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserRefreshTokenInsert;
	PRINT 'Procedura UserRefreshTokenInsert byla smazána.';
GO
CREATE PROCEDURE dbo.UserRefreshTokenInsert
	@UserId INT,
    @Token NVARCHAR(2000),
    @IsRevoked BIT,
	@Expires DATETIME,
    @CreateAuthorId INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO UserRefreshToken 
	(
        UserId,
		Token,
		IsRevoked,
		Expires,
		CreateDateTime,
		CreateAuthorId
    )
    VALUES 
	(
        @UserId,
		@Token,
		@IsRevoked,
		@Expires,
		SYSUTCDATETIME(),
		@CreateAuthorId
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO