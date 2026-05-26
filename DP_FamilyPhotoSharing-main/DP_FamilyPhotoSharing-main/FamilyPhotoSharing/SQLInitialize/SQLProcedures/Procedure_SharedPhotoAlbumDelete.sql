IF OBJECT_ID('dbo.SharedPhotoAlbumDelete', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.SharedPhotoAlbumDelete;
    PRINT 'Procedura SharedPhotoAlbumDelete byla smazána.';
END
GO

CREATE PROCEDURE dbo.SharedPhotoAlbumDelete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM SharedPhotoAlbum WHERE Id = @Id;
END
GO
