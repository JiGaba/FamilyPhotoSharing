IF OBJECT_ID('dbo.PhotoAlbumSelectByOwnerUserId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoAlbumSelectByOwnerUserId;
	PRINT 'Procedura PhotoAlbumSelectByOwnerUserId byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoAlbumSelectByOwnerUserId
    @OwnerUserId INT
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
      ,(SELECT U.UserName + ' ' + U.UserSurname FROM Users AS U WHERE U.Id = PA.CreateAuthor) AS CreateAuthorName
      ,(SELECT COUNT(*) FROM RelationAlbumPhoto AS RLP WHERE RLP.AlbumId = PA.Id) AS PhotoCount
    FROM 
	   PhotoAlbum AS PA
    WHERE
       OwnerUserId = @OwnerUserId AND Personal = 1
END
GO