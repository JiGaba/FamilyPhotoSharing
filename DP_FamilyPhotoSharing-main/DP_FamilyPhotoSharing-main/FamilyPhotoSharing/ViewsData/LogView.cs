using Microsoft.AspNetCore.Mvc.Rendering;
using ModelLayer.Data;

namespace FamilyPhotoSharing.ViewsData
{
    public class LogView
    {
        public bool IsUserSelected { get; set; }
        public int? SelectedGroupId { get; set; }
        public int? SelectedUserId { get; set; }
        public int? SelectedActionType { get; set; }
        public int? SelectedLogType { get; set; }
        public bool LastIsUserSelected { get; set; }
        public int LastSelectedGroupId { get; set; }
        public int LastSelectedUserId { get; set; }
        public int LastSelectedActionType { get; set; }
        public int LastSelectedLogType { get; set; }

        public List<SelectListItem> Groups { get; set; } = new();
        public List<SelectListItem> Users { get; set; } = new();
        public List<SelectListItem> ActionTypes { get; set; } = new();
        public List<SelectListItem> LogTypes { get; set; } = new();

        public List<SystemLogView> TableData { get; set; } = new();

        public int Offset { get; set; }
        public int Limit { get; set; } = 10;
        public int Total { get; set; }
    }
}
