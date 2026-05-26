IF OBJECT_ID('dbo.RelationAlbumPhotoInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.RelationAlbumPhotoInsert;
	PRINT 'Procedura RelationAlbumPhotoInsert byla smazána.';
GO
CREATE PROCEDURE dbo.RelationAlbumPhotoInsert
	@PhotoId INT,
    @GroupId INT,
    @AlbumId INT,
	@CreateAuthor INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO RelationAlbumPhoto 
	(
        AlbumId,
        GroupId,
		PhotoId,
        CreateAuthorId,
        CreateDateTime
    )
    VALUES 
	(
        @AlbumId,
        @GroupId,
		@PhotoId,
        @CreateAuthor,
        SYSUTCDATETIME()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO