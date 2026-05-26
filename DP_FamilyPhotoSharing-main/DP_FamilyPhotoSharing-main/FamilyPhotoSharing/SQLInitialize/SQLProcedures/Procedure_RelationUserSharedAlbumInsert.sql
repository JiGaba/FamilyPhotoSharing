IF OBJECT_ID('dbo.RelationUserSharedAlbumInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RelationUserSharedAlbumInsert;
	PRINT 'Procedura RelationUserSharedAlbumInsert byla smazána.';
GO
CREATE PROCEDURE dbo.RelationUserSharedAlbumInsert
	@UserId INT,
    @SharedAlbumId INT,
	@CreateAuthor INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO RelationUserSharedAlbum 
	(
        SharedAlbumId,
		UserId,
        CreateAuthorId,
        CreateDateTime
    )
    VALUES 
	(
        @SharedAlbumId,
		@UserId,
        @CreateAuthor,
        SYSUTCDATETIME()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO