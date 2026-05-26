namespace FamilyPhotoSharing.Requests
{
    public class LogRequest
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int? ActionType { get; set; }
        public int? LogType { get; set; }
        public int? UserId { get; set; }
        public int? GroupId { get; set; }
        public bool IsUserSelected { get; set; }
    }

}
