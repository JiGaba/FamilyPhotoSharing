IF OBJECT_ID('dbo.SharedPhotoAlbumSelect', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SharedPhotoAlbumSelect;
	PRINT 'Procedura SharedPhotoAlbumSelect byla smazána.';
GO
CREATE PROCEDURE dbo.SharedPhotoAlbumSelect
    @Id INT
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
      ,ISNULL(
        (SELECT STRING_AGG(T_RUSA.UserId, ',') 
         FROM RelationUserSharedAlbum AS T_RUSA 
         WHERE T_RUSA.SharedAlbumId = SPA.Id),
        ''
        ) AS HostUserIds
    FROM 
	   SharedPhotoAlbum AS SPA
    WHERE
       SPA.Id = @Id
END
GO