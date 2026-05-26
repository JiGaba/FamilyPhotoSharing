IF OBJECT_ID('dbo.PhotoDelete', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.PhotoDelete;
    PRINT 'Procedura PhotoDelete byla smazána.';
END
GO

CREATE PROCEDURE dbo.PhotoDelete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Photo WHERE Id = @Id;
END
GO
