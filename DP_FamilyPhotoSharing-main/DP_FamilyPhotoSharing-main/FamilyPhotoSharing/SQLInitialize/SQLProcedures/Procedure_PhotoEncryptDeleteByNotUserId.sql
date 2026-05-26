IF OBJECT_ID('dbo.PhotoEncryptDeleteByNotUserId', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.PhotoEncryptDeleteByNotUserId;
    PRINT 'Procedura PhotoEncryptDeleteByNotUserId byla smazána.';
END
GO

CREATE PROCEDURE dbo.PhotoEncryptDeleteByNotUserId
    @FileId INT,
    @FileType INT,
    @UserIds VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM PhotoEncrypt WHERE FileId = @FileId AND FileType = @FileType AND UserId NOT IN (
            SELECT value 
            FROM STRING_SPLIT(@UserIds, ',')
      );
END
GO
