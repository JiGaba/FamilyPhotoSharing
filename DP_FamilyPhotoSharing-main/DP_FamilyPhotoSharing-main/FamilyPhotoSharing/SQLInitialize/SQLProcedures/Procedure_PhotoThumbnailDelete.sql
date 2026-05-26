IF OBJECT_ID('dbo.PhotoThumbnailDelete', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.PhotoThumbnailDelete;
    PRINT 'Procedura PhotoThumbnailDelete byla smazána.';
END
GO

CREATE PROCEDURE dbo.PhotoThumbnailDelete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM PhotoThumbnail WHERE PhotoId = @Id;
END
GO
