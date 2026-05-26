IF OBJECT_ID('dbo.SharedPhotoAlbumSelectByOwnerUserId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SharedPhotoAlbumSelectByOwnerUserId;
	PRINT 'Procedura SharedPhotoAlbumSelectByOwnerUserId byla smazána.';
GO
CREATE PROCEDURE dbo.SharedPhotoAlbumSelectByOwnerUserId
    @OwnerUserId INT
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  Id
      ,AlbumName
      ,AlbumDescription
	  ,TitlePhotoId
      ,UserGroupsId
      ,OwnerUserId
      ,CreateAuthor
      ,dbo.ConvertUtcToCzechTime(CreateDateTime) AS CreateDateTime
      ,(SELECT U.UserName + ' ' + U.UserSurname FROM Users AS U WHERE U.Id = SPA.CreateAuthor) AS CreateAuthorName
      ,(SELECT COUNT(*) FROM RelationSharedAlbumPhoto AS RLP WHERE RLP.SharedAlbumId = SPA.Id) AS PhotoCount
      ,(SELECT COUNT(*) FROM RelationUserSharedAlbum AS RUSA WHERE RUSA.SharedAlbumId = SPA.Id) AS HostUserCount
    FROM 
	   SharedPhotoAlbum AS SPA
    WHERE
       OwnerUserId = @OwnerUserId
END
GO