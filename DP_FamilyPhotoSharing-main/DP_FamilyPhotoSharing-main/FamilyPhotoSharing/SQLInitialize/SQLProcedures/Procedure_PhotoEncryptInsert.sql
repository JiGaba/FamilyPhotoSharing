IF OBJECT_ID('dbo.PhotoEncryptInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoEncryptInsert;
	PRINT 'Procedura PhotoEncryptInsert byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoEncryptInsert
    @UserId INT,
    @FileId INT,
    @FileType SMALLINT,
    @Aes VARBINARY(MAX),
	@Nonce VARBINARY(12),
    @Tag VARBINARY(16),
	@CreateAuthorId INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO PhotoEncrypt 
	(
        UserId,
        FileId,
        FileType,
        Aes,
	    Nonce,
        Tag,
        CreateDateTime,     
        CreateAuthorId
    )
    VALUES 
	(
        @UserId,
        @FileId,
        @FileType,
        @Aes,
	    @Nonce,
        @Tag,
        SYSUTCDATETIME(),
	    @CreateAuthorId
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO