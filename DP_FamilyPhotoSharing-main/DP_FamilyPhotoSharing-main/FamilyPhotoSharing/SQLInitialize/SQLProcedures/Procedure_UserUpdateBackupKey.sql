IF OBJECT_ID('dbo.UserUpdateBackupKey', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UserUpdateBackupKey;
	PRINT 'Procedura UserUpdateBackupKey byla smazána.';
GO
CREATE PROCEDURE dbo.UserUpdateBackupKey
	@Id INT,
	@BackupKey NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET
		BackupKey = @BackupKey,
        CreateDateTime = SYSUTCDATETIME()
    WHERE
        Id = @Id;
END
GO