IF OBJECT_ID('dbo.SystemsLogSelectByParameters', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SystemsLogSelectByParameters;
	PRINT 'Procedura SystemsLogSelectByParameters byla smazána.';
GO
CREATE PROCEDURE dbo.SystemsLogSelectByParameters
    @GroupId INT = NULL,
    @FilterGroup NVARCHAR(10) = NULL,
    @UserId INT = NULL,
    @FilterUser NVARCHAR(10) = NULL,
    @LogType INT = NULL,
    @FilterLogType NVARCHAR(10) = NULL,
    @ActionType INT = NULL,
    @FilterActionType NVARCHAR(10) = NULL,
    @Offset INT = 0,
    @Limit INT = 20
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  Id
      ,LogType
      ,ActionType
      ,LogDescription
      ,CreateAuthorId
      ,GroupsId
      ,dbo.ConvertUtcToCzechTime(CreateDate) AS CreateDate
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
    ORDER BY Id DESC
    OFFSET @Offset ROWS
    FETCH NEXT @Limit ROWS ONLY;
END
GO