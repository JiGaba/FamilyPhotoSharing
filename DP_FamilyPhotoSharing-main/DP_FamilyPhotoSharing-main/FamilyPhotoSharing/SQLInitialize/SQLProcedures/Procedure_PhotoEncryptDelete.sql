IF OBJECT_ID('dbo.PhotoEncryptDelete', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.PhotoEncryptDelete;
    PRINT 'Procedura PhotoEncryptDelete byla smazána.';
END
GO

CREATE PROCEDURE dbo.PhotoEncryptDelete
    @FileId INT,
    @FileType INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM PhotoEncrypt WHERE FileId = @FileId AND FileType = @FileType;
END
GO
