IF OBJECT_ID('dbo.SystemLogSelectCountByParameters', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SystemLogSelectCountByParameters;
	PRINT 'Procedura SystemLogSelectCountByParameters byla smazána.';
GO
CREATE PROCEDURE dbo.SystemLogSelectCountByParameters
    @GroupId INT = NULL,
    @FilterGroup NVARCHAR(10) = NULL,
    @UserId INT = NULL,
    @FilterUser NVARCHAR(10) = NULL,
    @LogType INT = NULL,
    @FilterLogType NVARCHAR(10) = NULL,
    @ActionType INT = NULL,
    @FilterActionType NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  COUNT(*) AS TotalCount
    FROM 
	   SystemLog
    WHERE
       (@FilterGroup IS NULL OR GroupsId = @GroupId)
       AND
       (@FilterUser IS NULL OR CreateAuthorId = @UserId)
       AND
       (@FilterLogType IS NULL OR LogType = @LogType)
       AND
       (@FilterActionType IS NULL OR ActionType = @ActionType)
END
GO