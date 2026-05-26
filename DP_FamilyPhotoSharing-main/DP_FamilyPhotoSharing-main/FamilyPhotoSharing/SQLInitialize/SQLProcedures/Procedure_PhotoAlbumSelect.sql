IF OBJECT_ID('dbo.PhotoAlbumSelect', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoAlbumSelect;
	PRINT 'Procedura PhotoAlbumSelect byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoAlbumSelect
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  Id
      ,AlbumName
      ,AlbumDescription
      ,Personal
	  ,TitlePhotoId
      ,UserGroupsId
      ,OwnerUserId
      ,CreateAuthor
      ,dbo.ConvertUtcToCzechTime(CreateDateTime) AS CreateDateTime
    FROM 
	   PhotoAlbum
    WHERE
       Id = @Id
END
GO