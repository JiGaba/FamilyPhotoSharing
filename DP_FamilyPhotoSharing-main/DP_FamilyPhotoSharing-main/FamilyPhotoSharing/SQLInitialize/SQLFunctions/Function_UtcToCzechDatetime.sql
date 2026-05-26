IF EXISTS (
    SELECT 1 
    FROM sys.objects 
    WHERE object_id = OBJECT_ID('dbo.ConvertUtcToCzechTime') 
      AND type = 'FN'
)
BEGIN
    DROP FUNCTION dbo.ConvertUtcToCzechTime;
END

EXEC('
    CREATE FUNCTION dbo.ConvertUtcToCzechTime
    (
        @UtcDateTime datetime2
    )
    RETURNS datetime2
    AS
    BEGIN
        RETURN CAST(
            @UtcDateTime 
                AT TIME ZONE ''UTC''
                AT TIME ZONE ''Central European Standard Time''
            AS datetime2
        );
    END
');