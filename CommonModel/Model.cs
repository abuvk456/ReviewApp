using CommonModel;

namespace CommonModel
{
    public class UserCredentials
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
    public class Vote
    {
        public int ID { get; set; }
        public DateTime VotedDate { get; set; }
        public bool IsUpVote { get; set; }
        public int VotedBy { get; set; }
        public int VotedFor { get; set; }
    }
    public enum TopicType
    {
        Movie,
        TVShow,
        Other,
        All,
        My
    }
    public partial class Topic
    {
        public int TopicId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TopicType { get; set; }

        public TopicType Type { get; set; }
        public string? TopicImage { get; set; } = "";
        public string? TopicVideo { get; set; } = "";
        public int IMDBRating { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string? IDMBID { get; set; }
        public string? TMDBID { get; set; }
        public string? UsernName { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }

    }
    public partial class User
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int Age { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public int UpvoteCount { get; set; } = 0;
        public int DownvoteCount { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginDate { get; set; }
        public Session? SessionInfo { get; set; }
        public List<Topic>? Topics { get; set; }
        public List<WatchList>? WatchList { get; set; }
        public UserProfileInfo? UserProfileInfo { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + ' ' + LastName;
            }
        }

    }
    public class UserProfileInfo
    {
        public int TopicsCount { get; set; }
        public int FollowingsCount { get; set; }
        public int FollowersCount { get; set; }
        public int WatchListCount { get; set; }
    }

    public class WatchList
    {
        public int UserId { get; set; }
        public int TopicId { get; set; }
        public DateTime WatchedDateTime { get; set; }
        public Topic? Topic { get; set; }
    }
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int TopicId { get; set; }
        public string CommentText { get; set; }
        public int CommentedBy { get; set; }
        public bool IsEditAllowed { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public DateTime CommentDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public string UserName { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual User User { get; set; }
        
    }
    public class Session
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
    }
    public class Message
    {
        public int MessageID { get; set; }
        public int SentBy { get; set; }
        public string SentToName { get; set; }
        public int SentTo { get; set; }
        public string SentByName { get; set; }
        public DateTime SentDatetime { get; set; }
        public DateTime? ReadDatetime { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsFlaged { get; set; }
        public string MessageText { get; set; }
    }
    public class Notification
    {
        public int ID { get; set; }
        public string TypeOfNotification { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public int CreatedBy { get; set; }
        public int CreatedFor { get; set; }
        public string NotificationText { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
        public int? RelatedDataID { get; set; }
    }
    public class WatchlistEntry
    {
        public int UserID { get; set; }
        public int TopicID { get; set; }
        public DateTime WatchedDateTime { get; set; }

        public bool IsDeleteAble { get; set; } = false;
        public Topic Movie { get; set; }
    }

}
