IF OBJECT_ID('dbo.UserSelectCountByGroupId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserSelectCountByGroupId;
	PRINT 'Procedura UserSelectCountByGroupId byla smazána.';
GO
CREATE PROCEDURE dbo.UserSelectCountByGroupId
	@GroupId INT
AS
BEGIN
    SET NOCOUNT ON;
	    SELECT 
            Active,
            COUNT(*) AS UserCount
    FROM 
        Users
    WHERE 
        GroupId = @GroupId
    GROUP BY 
        Active;
END
GO