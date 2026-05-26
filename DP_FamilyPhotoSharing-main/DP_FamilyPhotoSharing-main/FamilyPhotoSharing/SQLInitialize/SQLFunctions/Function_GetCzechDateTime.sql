IF EXISTS (
    SELECT 1 
    FROM sys.objects 
    WHERE object_id = OBJECT_ID('dbo.GetCzechDateTime') 
      AND type = 'FN'
)
BEGIN
    DROP FUNCTION dbo.GetCzechDateTime;
END
GO

EXEC('
    CREATE FUNCTION dbo.GetCzechDateTime()
    RETURNS datetime2
    AS
    BEGIN
        RETURN SYSUTCDATETIME();
    END
');
GO