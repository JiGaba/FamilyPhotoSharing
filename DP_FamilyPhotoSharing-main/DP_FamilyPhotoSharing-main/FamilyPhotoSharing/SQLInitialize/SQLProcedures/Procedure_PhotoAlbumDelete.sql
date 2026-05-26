IF OBJECT_ID('dbo.PhotoAlbumDelete', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.PhotoAlbumDelete;
    PRINT 'Procedura PhotoAlbumDelete byla smazána.';
END
GO

CREATE PROCEDURE dbo.PhotoAlbumDelete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM PhotoAlbum WHERE Id = @Id;
END
GO
