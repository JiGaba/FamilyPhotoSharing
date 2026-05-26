IF OBJECT_ID('dbo.SharedPhotoAlbumSelectByHostUserId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SharedPhotoAlbumSelectByHostUserId;
	PRINT 'Procedura SharedPhotoAlbumSelectByHostUserId byla smazána.';
GO
CREATE PROCEDURE dbo.SharedPhotoAlbumSelectByHostUserId
    @HostUserId INT
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  SPA.Id
      ,SPA.AlbumName
      ,SPA.AlbumDescription
	  ,SPA.TitlePhotoId
      ,SPA.UserGroupsId
      ,SPA.OwnerUserId
      ,SPA.CreateAuthor
      ,dbo.ConvertUtcToCzechTime(SPA.CreateDateTime) AS CreateDateTime
      ,(SELECT U.UserName + ' ' + U.UserSurname FROM Users AS U WHERE U.Id = SPA.CreateAuthor) AS CreateAuthorName
      ,(SELECT COUNT(*) FROM RelationSharedAlbumPhoto AS RLP WHERE RLP.SharedAlbumId = SPA.Id) AS PhotoCount
      ,(SELECT COUNT(*) FROM RelationUserSharedAlbum AS T_RUSA WHERE T_RUSA.SharedAlbumId = SPA.Id) AS HostUserCount
    FROM 
	   RelationUserSharedAlbum AS RUSA
    LEFT JOIN
        SharedPhotoAlbum AS SPA ON RUSA.SharedAlbumId = SPA.Id
    WHERE
       RUSA.UserId = @HostUserId
END
GO