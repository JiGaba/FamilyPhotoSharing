IF OBJECT_ID('dbo.PhotoUpdate', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoUpdate;
	PRINT 'Procedura PhotoUpdate byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoUpdate
    @Id INT,
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
	UPDATE Photo SET
        PhotoName = @PhotoName,
        PhotoDescription = @PhotoDescription,
        OwnerId = @OwnerId,
		GroupsId = @GroupsId,
        FileSize = @FileSize,
        FSFileName = @FSFileName,
        Personal = @Personal,
        CreateDateTime = SYSUTCDATETIME(),     
        CreateAuthor = @CreateAuthor
    WHERE Id = @Id
END
GO