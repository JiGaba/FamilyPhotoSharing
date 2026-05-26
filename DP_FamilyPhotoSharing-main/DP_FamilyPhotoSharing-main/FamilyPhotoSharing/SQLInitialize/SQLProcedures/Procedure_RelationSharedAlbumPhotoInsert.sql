IF OBJECT_ID('dbo.RelationSharedAlbumPhotoInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RelationSharedAlbumPhotoInsert;
	PRINT 'Procedura RelationSharedAlbumPhotoInsert byla smazána.';
GO
CREATE PROCEDURE dbo.RelationSharedAlbumPhotoInsert
	@PhotoId INT,
    @SharedAlbumId INT,
	@CreateAuthor INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO RelationSharedAlbumPhoto 
	(
        SharedAlbumId,
		PhotoId,
        CreateAuthorId,
        CreateDateTime
    )
    VALUES 
	(
        @SharedAlbumId,
		@PhotoId,
        @CreateAuthor,
        SYSUTCDATETIME()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO