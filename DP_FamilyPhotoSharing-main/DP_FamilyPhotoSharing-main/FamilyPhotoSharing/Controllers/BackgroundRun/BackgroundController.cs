using FamilyPhotoSharing.Controllers.Base;
using FamilyPhotoSharing.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.BackgroundModels;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.PhotoEnums;
using ModelLayer.System;
using ServiceLayer.BackgroundServices;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.SharedPhotoAlbum;
using System.Text.RegularExpressions;

namespace FamilyPhotoSharing.Controllers.BackgroundRun
{
    [Route("Background")]
    public class BackgroundController : BaseController
    {
        private readonly IBackgroundQueue _queue;
        private readonly IBackgroundJobStore _store;
        private readonly ISharedPhotoAlbumService _sharedPhotoAlbumService;

        public BackgroundController(IMemoryCache cache, ISystemLogService log, IUserService userService, IBackgroundQueue queue, 
            IBackgroundJobStore store, ISharedPhotoAlbumService sharedPhotoAlbumService) : base(cache, log, userService)
        {
            _queue = queue;
            _store = store;
            _sharedPhotoAlbumService = sharedPhotoAlbumService;
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpPost("Start_MoveToMainGroup")]
        public IActionResult Start_MoveToMainGroup([FromBody] MoveToGroupRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            var job = new BackgroundJob
            {
                MoveToGroupBGModel = new MoveToGroupBGModel
                {
                    PhotoIds = request.PhotoIds,
                    AlbumId = request.AlbumId,
                    UserId = result.userData.Id,
                    GroupId = result.userData.GroupId,
                    ThumbnailData = request.ThumbnailData
                },
                JobType = JobTypeEnum.MoveToGroup,
                Total = request.PhotoIds.Count,
            };

            _store.Add(job);
            _queue.Enqueue(job);

            return Json(new { jobId = job.JobId });
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpPost("Start_MoveToMainPersonal")]
        public IActionResult Start_MoveToMainPersonal([FromBody] MoveToPersonalRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            var job = new BackgroundJob
            {
                MoveToPersonalBGModel = new MoveToPersonalBGModel
                {
                    PhotoIds = request.PhotoIds,
                    AlbumId = request.AlbumId,
                    UserId = result.userData.Id,
                    GroupId = result.userData.GroupId,
                    ThumbnailData = request.ThumbnailData
                },
                JobType = JobTypeEnum.MoveToPersonal,
                Total = request.PhotoIds.Count,
            };

            _store.Add(job);
            _queue.Enqueue(job);

            return Json(new { jobId = job.JobId });
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpPost("Start_PhotoDelete")]
        public IActionResult Start_PhotoDelete([FromBody] PhotoDeleteRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            var job = new BackgroundJob
            {
                PhotoDeleteBGModel = new PhotoDeleteBGModel
                {
                    PhotoIds = request.PhotoIds,
                    AlbumId = request.AlbumId,
                    UserId = result.userData.Id,
                    GroupId = result.userData.GroupId,
                    Folder = result.userData.Folder,
                    ThumbnailData = request.ThumbnailData
                },
                JobType = JobTypeEnum.PhotoDelete,
                Total = request.PhotoIds.Count,
            };

            _store.Add(job);
            _queue.Enqueue(job);

            return Json(new { jobId = job.JobId });
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpPost("Start_AddPhotoToSharedAlbum")]
        public IActionResult Start_AddPhotoToSharedAlbum([FromBody] AddPhotoToSharedAlbumRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            var job = new BackgroundJob
            {
                AddPhotoToSharedAlbumBGModel = new AddPhotoToSharedAlbumBGModel
                {
                    PhotoIds = request.PhotoIds,
                    SharedAlbumId = request.AlbumId,
                    UserId = result.userData.Id,
                    GroupId = result.userData.GroupId,
                    UserRoleEnum = result.userData.RoleEnum,
                },
                JobType = JobTypeEnum.AddPhotoToSharedAlbum,
                Total = request.PhotoIds.Count,
            };

            _store.Add(job);
            _queue.Enqueue(job);

            return Json(new { jobId = job.JobId });
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpPost("Start_RemoveFromSharedAlbum")]
        public IActionResult Start_RemoveFromSharedAlbum([FromBody] RemoveFromSharedAlbumRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            var job = new BackgroundJob
            {
                RemoveFromSharedAlbumBGModel = new RemoveFromSharedAlbumBGModel
                {
                    PhotoIds = request.PhotoIds,
                    AlbumId = request.AlbumId,
                    UserId = result.userData.Id,
                    GroupId = result.userData.GroupId,
                    ThumbnailData = request.ThumbnailData
                },
                JobType = JobTypeEnum.RemoveFromSharedAlbum,
                Total = request.PhotoIds.Count,
            };

            _store.Add(job);
            _queue.Enqueue(job);

            return Json(new { jobId = job.JobId });
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpPost("Start_DeleteSharedAlbum")]
        public IActionResult Start_DeleteSharedAlbum([FromBody] DeleteAlbumRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            var job = new BackgroundJob
            {
                SharedPhotoAlbumDeleteBGModel = new SharedPhotoAlbumDeleteBGModel
                {
                    AlbumId = request.AlbumId,
                    UserId = result.userData.Id,
                    GroupId = result.userData.GroupId
                },
                JobType = JobTypeEnum.SharedPhotoAlbumDelete,
                Total = 1 // upraví se v processoru podle počtu fotografií
            };

            _store.Add(job);
            _queue.Enqueue(job);

            return Json(new { jobId = job.JobId });
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpPost("Start_DeleteAlbum")]
        public IActionResult Start_DeleteAlbum([FromBody] DeleteAlbumRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            var job = new BackgroundJob
            {
                PhotoAlbumDeleteBGModel = new PhotoAlbumDeleteBGModel
                {
                    AlbumId = request.AlbumId,
                    UserId = result.userData.Id,
                    GroupId = result.userData.GroupId,
                },
                JobType = JobTypeEnum.PhotoAlbumDelete,
                Total = 1 // upraví se v processoru podle počtu fotografií
            };

            _store.Add(job);
            _queue.Enqueue(job);

            return Json(new { jobId = job.JobId });
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpPost("Start_ActiveUser")]
        public IActionResult Start_ActiveUser([FromBody] ActiveUserRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            var job = new BackgroundJob
            {
                AddUserToGroupBGModel = new AddUserToGroupBGModel
                {
                    UserId = request.UserId,
                    GroupId = result.userData.GroupId,
                    AuthorId = result.userData.Id,
                    RoleEnum = result.userData.RoleEnum,
                },
                JobType = JobTypeEnum.AddUserToGroup,
                Total = 1 // upraví se v processoru podle počtu fotografií
            };

            _store.Add(job);
            _queue.Enqueue(job);

            return Json(new { jobId = job.JobId });
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpPost("Start_ChangeUsersSharedAlbum")]
        public async Task<IActionResult> Start_ChangeUsersSharedAlbum([FromBody] ChangeUsersSharedAlbumRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            // připravit list přidávaných a odebíraných uživatelů
            var album = await _sharedPhotoAlbumService.Get(request.AlbumId);
            var usersActual = album?.HostUserIdsList;
            usersActual ??= new List<int>();
            usersActual.Add(album.OwnerUserId);

            _LoadUsersIdList(usersActual, request.UserIds);

            var job = new BackgroundJob
            {
                ChangeUserSharedAlbumBGModel = new ChangeUserSharedAlbumBGModel
                {
                    UserId = result.userData.Id,
                    GroupId = result.userData.GroupId,
                    UserIdAdd = _usr_AddedIds,
                    UserIdRemove = _usr_RemovedIds,
                    UserRoleEnum = result.userData.RoleEnum,
                    AlbumId = request.AlbumId
                },
                JobType = JobTypeEnum.ChangeUserSharedAlbum,
                Total = 1 // upraví se v processoru podle počtu fotografií
            };

            _store.Add(job);
            _queue.Enqueue(job);

            return Json(new { jobId = job.JobId });
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpGet("Status/{jobId}")]
        public IActionResult Status(Guid jobId)
        {
            var job = _store.Get(jobId);
            if (job == null)
            {
                return NotFound();
            }

            var percent = job.Total == 0 ? 0 : (int)((double)job.Processed / job.Total * 100);

            if (percent == 100)
                CacheInvalidateAfterJob(jobId);

            Console.WriteLine($"CONTROLLER: {job.Processed}/{job.Total} ({percent}%)");
            return Json(new
            {
                job.Status,
                job.Processed,
                job.Total,
                Percent = percent,
                job.Error
            });
        }

        private async Task CacheInvalidateAfterJob(Guid jobId)
        {
            var job = _store.Get(jobId);
            var key = "";

            if (job != null)
            {
                switch (job.JobType)
                {
                    case JobTypeEnum.PhotoDelete:
                        if(job.PhotoDeleteBGModel != null)
                            key = _CacheKeyTotalImages(job.PhotoDeleteBGModel.ThumbnailData, job.PhotoDeleteBGModel.UserId, job.PhotoDeleteBGModel.GroupId, job.PhotoDeleteBGModel.AlbumId);
                        break;
                    case JobTypeEnum.MoveToPersonal:
                        if (job.MoveToPersonalBGModel != null)
                        {
                            key = _CacheKeyTotalImages(job.MoveToPersonalBGModel.ThumbnailData, job.MoveToPersonalBGModel.UserId, job.MoveToPersonalBGModel.GroupId, job.MoveToPersonalBGModel.AlbumId);
                            var users = await _userService.SelectUserByGroupIdActiveRoleId(job.MoveToPersonalBGModel.GroupId, true, new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.GroupAdmin, UserRoleEnum.User, UserRoleEnum.Host});
                            
                            foreach (var user in users)
                            {
                                var keyTemp = _CacheKeyTotalImages(ThumbnailData.MainPersonalGallery, user.Id, job.MoveToPersonalBGModel.GroupId, job.MoveToPersonalBGModel.AlbumId);
                                _CacheInvalidate(keyTemp);
                            }
                        }
                        break;
                    case JobTypeEnum.MoveToGroup:
                        if (job.MoveToGroupBGModel != null)
                        {
                            key = _CacheKeyTotalImages(job.MoveToGroupBGModel.ThumbnailData, job.MoveToGroupBGModel.UserId, job.MoveToGroupBGModel.GroupId, job.MoveToGroupBGModel.AlbumId);
                            var users = await _userService.SelectUserByGroupIdActiveRoleId(job.MoveToGroupBGModel.GroupId, true, new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.GroupAdmin, UserRoleEnum.User, UserRoleEnum.Host });

                            foreach (var user in users)
                            {
                                var keyTemp = _CacheKeyTotalImages(ThumbnailData.MainGroupGallery, user.Id, job.MoveToGroupBGModel.GroupId, 0);
                                _CacheInvalidate(keyTemp);
                                var keyTemp2 = _CacheKeyTotalImages(ThumbnailData.MainGroupGalleryOwnPhoto, user.Id, job.MoveToGroupBGModel.GroupId, 0);
                                _CacheInvalidate(keyTemp2);
                            }
                        }
                        break;
                    case JobTypeEnum.AddPhotoToSharedAlbum:
                        if (job.AddPhotoToSharedAlbumBGModel != null)
                            key = _CacheKeyTotalImages(ThumbnailData.MySharedAlbum, job.AddPhotoToSharedAlbumBGModel.UserId, job.AddPhotoToSharedAlbumBGModel.GroupId, job.AddPhotoToSharedAlbumBGModel.SharedAlbumId);
                        break;
                    case JobTypeEnum.RemoveFromSharedAlbum:
                        if (job.RemoveFromSharedAlbumBGModel != null)
                            key = _CacheKeyTotalImages(ThumbnailData.MySharedAlbum, job.RemoveFromSharedAlbumBGModel.UserId, job.RemoveFromSharedAlbumBGModel.GroupId, job.RemoveFromSharedAlbumBGModel.AlbumId);
                        break;
                } 
            }

            _CacheInvalidate(key);
        }
    }

}
