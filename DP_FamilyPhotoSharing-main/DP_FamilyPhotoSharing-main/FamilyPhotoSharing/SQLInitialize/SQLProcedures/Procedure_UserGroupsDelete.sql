IF OBJECT_ID('dbo.UserGroupsDelete', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.UserGroupsDelete;
    PRINT 'Procedura UserGroupsDelete byla smazána.';
END
GO

CREATE PROCEDURE dbo.UserGroupsDelete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Users WHERE GroupId = @Id)
    BEGIN
        THROW 51001, 'Uživatelská skupina nelze smazat, není prázdná!', 1;
    END

    DELETE FROM UserGroups WHERE Id = @Id;
END
GO
