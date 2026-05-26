IF OBJECT_ID('dbo.SystemImagesInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SystemImagesInsert;
	PRINT 'Procedura SystemImagesInsert byla smazána.';
GO
CREATE PROCEDURE dbo.SystemImagesInsert
	@Size INT,
    @FSPhotoName NVARCHAR(255),
    @PhotoNameOriginal NVARCHAR(255),
    @CreateAuthorId INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO SystemImages
	(
        Size
        ,FSPhotoName
        ,PhotoNameOriginal
        ,CreateAuthorId
        ,CreateDateTime
    )
    VALUES 
	(
        @Size
        ,@FSPhotoName
        ,@PhotoNameOriginal
        ,@CreateAuthorId
        ,SYSUTCDATETIME()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO