IF OBJECT_ID('dbo.SystemImagesDelete', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.SystemImagesDelete;
    PRINT 'Procedura SystemImagesDelete byla smazána.';
END
GO

CREATE PROCEDURE dbo.SystemImagesDelete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM SystemImages WHERE Id = @Id;
END
GO
