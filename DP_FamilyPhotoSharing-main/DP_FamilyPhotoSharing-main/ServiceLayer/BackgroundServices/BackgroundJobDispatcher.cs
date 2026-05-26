using Microsoft.Extensions.DependencyInjection;
using ServiceLayer.BackgroundInterfaces;
using ServiceLayer.BackgroundProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.BackgroundServices
{
    public class BackgroundJobDispatcher
    {
        private readonly IServiceProvider _provider;

        public BackgroundJobDispatcher(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IJobProcessor ResolveProcessor(JobTypeEnum type)
        {
            return type switch
            {
                JobTypeEnum.PhotoDelete => _provider.GetRequiredService<PhotoDeleteProcessor>(),
                JobTypeEnum.MoveToPersonal => _provider.GetRequiredService<MoveToPersonalProcessor>(),
                JobTypeEnum.MoveToGroup => _provider.GetRequiredService<MoveToGroupProcessor>(),
                JobTypeEnum.AddUserToGroup => _provider.GetRequiredService<AddUserToGroupProcessor>(),
                JobTypeEnum.ChangeUserSharedAlbum => _provider.GetRequiredService<ChangeUserSharedAlbumProcessor>(),
                JobTypeEnum.AddPhotoToSharedAlbum => _provider.GetRequiredService<AddPhotoToSharedAlbumProcessor>(),
                JobTypeEnum.RemoveFromSharedAlbum => _provider.GetRequiredService<RemoveFromSharedAlbumProcessor>(),
                JobTypeEnum.SharedPhotoAlbumDelete => _provider.GetRequiredService<SharedPhotoAlbumDeleteProcessor>(),
                JobTypeEnum.PhotoAlbumDelete => _provider.GetRequiredService<PhotoAlbumDeleteProcessor>(),
                _ => throw new NotSupportedException("Neznámý typ operace.")
            };
        }
    }

}
