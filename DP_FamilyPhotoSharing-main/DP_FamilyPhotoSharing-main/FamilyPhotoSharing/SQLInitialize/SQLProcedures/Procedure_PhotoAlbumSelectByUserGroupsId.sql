IF OBJECT_ID('dbo.PhotoAlbumSelectByUserGroupsId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoAlbumSelectByUserGroupsId;
	PRINT 'Procedura PhotoAlbumSelectByUserGroupsId byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoAlbumSelectByUserGroupsId
    @UserGroupsId INT
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
      ,(SELECT UserName + ' ' + UserSurname FROM Users AS U WHERE U.Id = PA.CreateAuthor) AS CreateAuthorName
      ,(SELECT COUNT(*) FROM RelationAlbumPhoto AS RLP WHERE RLP.AlbumId = PA.Id) AS PhotoCount
    FROM 
	   PhotoAlbum AS PA
    WHERE
       UserGroupsId = @UserGroupsId AND Personal = 0
END
GO