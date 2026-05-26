IF OBJECT_ID('dbo.SystemLogInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SystemLogInsert;
	PRINT 'Procedura SystemLogInsert byla smazána.';
GO
CREATE PROCEDURE dbo.SystemLogInsert
	@LogType INT,
    @ActionType INT,
    @LogDescription NVARCHAR(MAX),
    @CreateAuthorId INT,
    @GroupsId INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO SystemLog 
	(
        LogType
        ,ActionType
        ,LogDescription
        ,CreateAuthorId
        ,GroupsId
        ,CreateDate
    )
    VALUES 
	(
        @LogType
        ,@ActionType
        ,@LogDescription
        ,@CreateAuthorId
        ,@GroupsId
        ,SYSUTCDATETIME()
    );
END
GO