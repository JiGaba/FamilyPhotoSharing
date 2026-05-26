IF OBJECT_ID('dbo.UserKeysSelectByUserId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserKeysSelectByUserId;
	PRINT 'Procedura UserKeysSelectByUserId byla smazána.';
GO
CREATE PROCEDURE dbo.UserKeysSelectByUserId
	@UserId INT
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  Id
      ,UserId
	  ,RSAPublicKey
	  ,RSAPrivateKey
	  ,PublicKeyNonce
	  ,PublicKeyTag
	  ,PrivateKeyNonce
	  ,PrivateKeyTag
      ,dbo.ConvertUtcToCzechTime(CreateDateTime) AS CreateDateTime
      ,CreateAuthorId
  FROM 
	UserKeys
  WHERE
	UserId = @UserId
END
GO