IF OBJECT_ID('dbo.PhotoEncryptSelectByUserIdPhotoId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.PhotoEncryptSelectByUserIdPhotoId;
	PRINT 'Procedura PhotoEncryptSelectByUserIdPhotoId byla smazána.';
GO
CREATE PROCEDURE dbo.PhotoEncryptSelectByUserIdPhotoId
	@UserId INT
    ,@FileId INT
    ,@FileType SMALLINT
AS
BEGIN
    SET NOCOUNT ON;
	SELECT
	  Id
      ,UserId
      ,FileId
      ,CAST(FileType AS SMALLINT) AS FileType
      ,Aes
	  ,Nonce
      ,Tag
      ,dbo.ConvertUtcToCzechTime(CreateDateTime) AS CreateDateTime     
      ,CreateAuthorId
  FROM 
	PhotoEncrypt
  WHERE
	UserId = @UserId AND FileId = @FileId AND FileType = @FileType
END
GO