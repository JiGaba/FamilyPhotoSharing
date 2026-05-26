IF OBJECT_ID('dbo.PhotoInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoInsert;
	PRINT 'Procedura PhotoInsert byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoInsert
	@PhotoName NVARCHAR(255),
    @PhotoDescription Text,
    @OwnerId INT,
	@GroupsId INT,
    @FileSize INT,
    @FSFileName NVARCHAR(255),
    @Personal BIT,
	@CreateAuthor INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO Photo 
	(
        PhotoName,
        PhotoDescription,
        OwnerId,
		GroupsId,
        FileSize,
        FSFileName,
        Personal,
        CreateDateTime,     
        CreateAuthor
    )
    VALUES 
	(
        @PhotoName,
        @PhotoDescription,
        @OwnerId,
		@GroupsId,
        @FileSize,
        @FSFileName,
        @Personal,
        SYSUTCDATETIME(),     
        @CreateAuthor
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO