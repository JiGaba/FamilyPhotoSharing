IF OBJECT_ID('dbo.PhotoThumbnailInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoThumbnailInsert;
	PRINT 'Procedura PhotoThumbnailInsert byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoThumbnailInsert
    @PhotoId INT,
	@FileSize INT,
	@FSThumbnailName NVARCHAR(255),
	@CreateAuthor INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO PhotoThumbnail
	(
        PhotoId,
        FileSize,
        FSThumbnailName,
        CreateDateTime,     
        CreateAuthor
    )
    VALUES 
	(
        @PhotoId,
        @FileSize,
        @FSThumbnailName,
        SYSUTCDATETIME(),     
        @CreateAuthor
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO