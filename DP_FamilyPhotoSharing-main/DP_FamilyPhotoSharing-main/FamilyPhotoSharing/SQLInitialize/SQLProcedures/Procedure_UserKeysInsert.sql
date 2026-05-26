IF OBJECT_ID('dbo.UserKeysInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserKeysInsert;
	PRINT 'Procedura UserKeysInsert byla smazána.';
GO
CREATE PROCEDURE dbo.UserKeysInsert
    @UserId INT,
	@RSAPublicKey VARBINARY(MAX),
	@RSAPrivateKey VARBINARY(MAX),
	@PublicKeyNonce VARBINARY(12),
	@PublicKeyTag VARBINARY(16),
	@PrivateKeyNonce VARBINARY(12),
	@PrivateKeyTag VARBINARY(16),
	@CreateAuthorId INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO UserKeys
	(
        UserId,
		RSAPublicKey,
		RSAPrivateKey,
		PublicKeyNonce,
		PublicKeyTag,
		PrivateKeyNonce,
		PrivateKeyTag,
        CreateDateTime,
        CreateAuthorId
    )
    VALUES 
	(
        @UserId,
		@RSAPublicKey,
		@RSAPrivateKey,
		@PublicKeyNonce,
		@PublicKeyTag,
		@PrivateKeyNonce,
		@PrivateKeyTag,
        SYSUTCDATETIME(),    
		@CreateAuthorId
    );
	SELECT CAST(SCOPE_IDENTITY() AS INT)
END
GO