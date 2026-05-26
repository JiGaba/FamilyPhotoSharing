using DataAccessLayer.Dao;
using DataAccessLayer.Dao.Accounts;
using DataAccessLayer.Dao.Groups;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dao.Photo;
using DataAccessLayer.Dao.PhotoAlbum;
using DataAccessLayer.Dao.PhotoEncrypt;
using DataAccessLayer.Dao.PhotoThumbnailDao;
using DataAccessLayer.Dao.RelationAlbumPhoto;
using DataAccessLayer.Dao.RelationSharedAlbumPhoto;
using DataAccessLayer.Dao.RelationUserSharedAlbum;
using DataAccessLayer.Dao.SharedPhotoAlbum;
using DataAccessLayer.Dao.SystemImages;
using DataAccessLayer.Dao.UserKeys;
using DataAccessLayer.Dao.UserRefreshToken;
using DataAccessLayer.Dto.Accounts;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Transactions;
using EncryptionLayer.Photo;
using FileAccessLayer;
using FileAccessLayer.DbInit;
using ModelLayer.Data;
using ServiceLayer.BackgroundProcessors;
using ServiceLayer.BackgroundServices;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.DbInitFileLoaderService;
using ServiceLayer.Services.Files;
using ServiceLayer.Services.Gallery;
using ServiceLayer.Services.Groups;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.PhotoAlbum;
using ServiceLayer.Services.PhotoEncrypt;
using ServiceLayer.Services.Photos;
using ServiceLayer.Services.PhotoThumbnail;
using ServiceLayer.Services.SharedPhotoAlbum;
using ServiceLayer.Services.SystemImages;
using ServiceLayer.Services.UserKeys;
using ServiceLayer.Services.UserRefreshToken;

namespace FamilyPhotoSharing.Compossition
{
    public class CompossitionRoot
    {
        public CompossitionRoot() { }

        public WebApplicationBuilder Build(WebApplicationBuilder builder)
        {
            // Services
            builder.Services.AddTransient<ITestService, TestService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IUploadFileService, UploadFileService>();
            builder.Services.AddTransient<IGroupService, GroupService>();
            builder.Services.AddTransient<IPhotoService, PhotoService>();
            builder.Services.AddTransient<ISystemLogService, SystemLogService>();
            builder.Services.AddTransient<ISystemImagesService, SystemImagesService>();
            builder.Services.AddTransient<IPhotoAlbumService, PhotoAlbumService>();
            builder.Services.AddTransient<IPhotoThumbnailService, PhotoThumbnailService>();
            builder.Services.AddTransient<IUserKeysService, UserKeysService>();
            builder.Services.AddTransient<IPhotoEncryptService, PhotoEncryptService>();
            builder.Services.AddTransient<IDbInitFileLoaderService, DbInitFileLoaderService>();
            builder.Services.AddTransient<IDownloadFileService, DownloadFileService>();
            builder.Services.AddTransient<ISharedPhotoAlbumService, SharedPhotoAlbumService>();
            builder.Services.AddTransient<IGalleryService, GalleryService>();
            builder.Services.AddTransient<IDeleteFileService, DeleteFileService>();
            builder.Services.AddTransient<IRefreshTokenService, RefreshTokenService>();

            // Transactions
            builder.Services.AddTransient<IUserTransaction, UserTransaction>();
            builder.Services.AddTransient<IDatabaseInitTransaction, DatabaseInitTransaction>();
            builder.Services.AddTransient<IPhotoTransaction, PhotoTransaction>();
            builder.Services.AddTransient<IGalleryTransaction, GalleryTransaction>();
            builder.Services.AddTransient<IPhotoAlbumTransaction, PhotoAlbumTransaction>();

            // Dao
            builder.Services.AddTransient<ITestDao, TestDao>();
            builder.Services.AddTransient<IUserDao, UserDao>();
            builder.Services.AddTransient<IUserGroupDao, UserGroupDao>();
            builder.Services.AddTransient<IPhotoDao, PhotoDao>();
            builder.Services.AddTransient<ISystemLogDao, SystemLogDao>();
            builder.Services.AddTransient<ISystemImageDao, SystemImageDao>();
            builder.Services.AddTransient<IPhotoAlbumDao, PhotoAlbumDao>();
            builder.Services.AddTransient<IPhotoEncryptDao, PhotoEncryptDao>();
            builder.Services.AddTransient<IUserKeysDao, UserKeysDao>();
            builder.Services.AddTransient<IPhotoThumbnailDao, PhotoThumbnailDao>();
            builder.Services.AddTransient<ISharedPhotoAlbumDao, SharedPhotoAlbumDao>();
            builder.Services.AddTransient<IRelationAlbumPhotoDao, RelationAlbumPhotoDao>();
            builder.Services.AddTransient<IRelationSharedAlbumPhotoDao, RelationSharedAlbumPhotoDao>();
            builder.Services.AddTransient<IRelationUserSharedAlbumDao, RelationUserSharedAlbumDao>();
            builder.Services.AddTransient<IUserRefreshTokenDao, UserRefreshTokenDao>();

            builder.Services.AddTransient<IInsertReturnIdentityTransaction<UserInsert>, UserDao>();
            builder.Services.AddTransient<IUpdateRowAttachedTransaction<UserUpdate>, UserDao>();

            // FileAccess
            builder.Services.AddTransient<IUploadFile, UploadFile>();
            builder.Services.AddTransient<IDownloadFile, DownloadFile>();
            builder.Services.AddTransient<IDeleteFile, DeleteFile>();
            builder.Services.AddTransient<IDbInitFileLoader, DbInitFileLoader>();

            // Encryprion
            builder.Services.AddTransient<ICryptoService, CryptoService>();
            builder.Services.AddTransient<IAesGcmCipher, AesGcmCipher>();
            builder.Services.AddTransient<IRsaCipher, RsaCipher>();

            // BackgroundServices
            builder.Services.AddSingleton<IBackgroundQueue, BackgroundQueue>();
            builder.Services.AddSingleton<IBackgroundJobStore, BackgroundJobStore>();
            builder.Services.AddSingleton<BackgroundJobDispatcher>();
            builder.Services.AddHostedService<BackgroundWorker>();

            // Processors
            builder.Services.AddTransient<PhotoDeleteProcessor>();
            builder.Services.AddTransient<MoveToPersonalProcessor>();
            builder.Services.AddTransient<MoveToGroupProcessor>();
            builder.Services.AddTransient<AddUserToGroupProcessor>();
            builder.Services.AddTransient<ChangeUserSharedAlbumProcessor>();
            builder.Services.AddTransient<AddPhotoToSharedAlbumProcessor>();
            builder.Services.AddTransient<RemoveFromSharedAlbumProcessor>();
            builder.Services.AddTransient<PhotoAlbumDeleteProcessor>();
            builder.Services.AddTransient<SharedPhotoAlbumDeleteProcessor>();

            return builder;
        }
    }
}
